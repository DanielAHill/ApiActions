#region Copyright
// Copyright (c) 2016 Daniel Alan Hill. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DanielAHill.AspNet.ApiActions.AbstractModeling;
using DanielAHill.AspNet.ApiActions.Authorization;
using DanielAHill.AspNet.ApiActions.Responses;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DanielAHill.AspNet.ApiActions
{
    public abstract class ApiAction : IApiAction
    {
        // ReSharper disable once MemberCanBePrivate.Global
        protected AbstractModel AbstractModel { get; private set; }
        protected HttpRequest HttpRequest { get { return _httpContext.Request; } }
        protected ClaimsPrincipal User { get { return _httpContext.User; } }
        protected ConnectionInfo Connection { get { return _httpContext.Connection; } }

        /// <summary>
        /// Gets the response of an action, following each execution step, if one exists
        /// </summary>
        /// <value>
        /// The response to send the client
        /// </value>
        public ApiActionResponse Response { get; protected set; }

        private IAuthFilterProvider _apiActionAuthorizationProvider;
        private HttpContext _httpContext;
        private IApiActionResponseAbstractFactory _responseAbstractFactory;

        #region Initialization
        public Task InitializeAsync(IApiActionInitializationContext initializationContext, CancellationToken cancellationToken)
        {
            if (initializationContext == null) throw new ArgumentNullException(nameof(initializationContext));
            InternalInitialize(initializationContext);

            // InitializeAsync Items
            return AppendedInitializeAsync(initializationContext, cancellationToken);
        }

        // ReSharper disable once MemberCanBeProtected.Global
        protected internal virtual void InternalInitialize(IApiActionInitializationContext initializationContext)
        {
            AbstractModel = initializationContext.AbstractModel ?? new AbstractModel();
            _httpContext = initializationContext.HttpContext;
            _apiActionAuthorizationProvider = _httpContext.RequestServices.GetRequiredService<IAuthFilterProvider>();
            _responseAbstractFactory = _httpContext.RequestServices.GetRequiredService<IApiActionResponseAbstractFactory>();
        }

        protected virtual Task AppendedInitializeAsync(IApiActionInitializationContext initializationContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
        #endregion

        #region AuthorizeAsync Route Access
        public virtual async Task AuthorizeAsync(CancellationToken cancellationToken)
        {
            var authorizationAttributes = _apiActionAuthorizationProvider.Get(GetType());

            if (authorizationAttributes == null || authorizationAttributes.Length == 0)
            {
                return;
            }

            var results = await Task.WhenAll(authorizationAttributes.Select(a => a.AuthorizeAsync(_httpContext, AbstractModel, cancellationToken))).ConfigureAwait(false);
            Response = Response ?? results.FirstOrDefault(r => r != null);
        }
        #endregion

        #region Data Pre-Load
        public virtual Task PreloadDataAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
        #endregion

        #region AuthorizeAsync Model Access
        public virtual Task AuthorizeDataAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
        #endregion

        #region ExecuteAsync
        public abstract Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken);
        #endregion

        #region Response Helpers

        protected ApiActionResponse Respond()
        {
            return Respond(NoContentResponse.Singleton);
        }

        protected ApiActionResponse Respond(HttpStatusCode statusCode)
        {
            return Respond(new StatusCodeResponse(statusCode));
        }

        protected ApiActionResponse Respond(int statusCode)
        {
            return Respond(new StatusCodeResponse(statusCode));
        }

        protected ApiActionResponse Respond<T>(T data)
        {
            var response = _responseAbstractFactory.Create(typeof(T))?.Create(data, typeof(T));

            if (response == null)
            {
                throw new InvalidOperationException($"No responses could be generated for type {typeof(T)}. Please explicitly state inherited type using generic parameter or register custom IApiActionResponseFactory into dependency injection.");
            }

            return Respond(response);
        }

        protected ApiActionResponse Respond<T>(HttpStatusCode statusCode, T data)
        {
            return Respond<T>((int) statusCode, data);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected ApiActionResponse Respond<T>(int statusCode, T data)
        {
            var response = _responseAbstractFactory.Create(typeof(T))?.Create(statusCode, data, typeof (T));

            if (response == null)
            {
                throw new InvalidOperationException($"No responses could be generated for type {typeof (T)}. Please explicitly state inherited type using generic parameter or register custom IApiActionResponseFactory into dependency injection.");
            }

            return Respond(response);
        }

        protected ApiActionResponse Respond(Stream data, string contentType)
        {
            return Respond(new StreamResponse(data, contentType));
        }

        protected ApiActionResponse Respond(int statusCode, Stream data, string contentType)
        {
            return Respond(new StreamResponse(statusCode, data, contentType));
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected ApiActionResponse Respond(ApiActionResponse response)
        {
            Response = response;
            return response;
        }

        #endregion
    }

    public abstract class ApiAction<TRequest> : ApiAction, IRequestModelApiAction
        where TRequest : class, new()
    {
        protected TRequest Data { get; private set; }
        
        protected internal override void InternalInitialize(IApiActionInitializationContext initializationContext)
        {
            base.InternalInitialize(initializationContext);
            Data = initializationContext.HttpContext.RequestServices.GetRequiredService<IRequestModelFactory>().Create<TRequest>(initializationContext.AbstractModel);
        }

        [Response(400, typeof(BadRequestDetails), "Bad Request - Input Validation Failed")]
        public Task ValidateModelAsync(CancellationToken cancellationToken)
        {
            var modelErrors = new List<ValidationResult>();
            if (Validator.TryValidateObject(Data, new ValidationContext(Data), modelErrors) && modelErrors.Count == 0)
            {
                return Task.FromResult(true);
            }

            Respond(modelErrors);

            return Task.FromResult(false);
        }

        public Task ValidateModelDataAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}