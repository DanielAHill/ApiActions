// Copyright (c) 2018 Daniel A Hill. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ApiActions.AbstractModeling;
using ApiActions.Authorization;
using ApiActions.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ApiActions
{
    public abstract class ApiAction<TRequest> : IApiAction
        where TRequest : class, new()
    {
        // ReSharper disable once MemberCanBePrivate.Global
        protected AbstractModel AbstractModel { get; private set; }
        protected TRequest Data { get; private set; }
        protected HttpRequest HttpRequest => _httpContext.Request;
        protected ClaimsPrincipal User => _httpContext.User;
        protected ConnectionInfo Connection => _httpContext.Connection;
        protected CancellationToken RequestAborted => _httpContext.RequestAborted;

        /// <inheritdoc />
        /// <summary>
        /// Gets the response of an action, following each execution step, if one exists
        /// </summary>
        /// <value>
        /// The response to send the client
        /// </value>
        public ApiActionResponse ActionResponse { get; protected set; }

        private IAuthFilterProvider _apiActionAuthorizationProvider;
        private HttpContext _httpContext;
        private IApiActionResponseAbstractFactory _responseAbstractFactory;

        #region Initialization

        public Task InitializeAsync(IApiActionInitializationContext initializationContext,
            CancellationToken cancellationToken)
        {
            if (initializationContext == null) throw new ArgumentNullException(nameof(initializationContext));

            AbstractModel = initializationContext.AbstractModel ?? new AbstractModel();
            _httpContext = initializationContext.HttpContext;
            _apiActionAuthorizationProvider = _httpContext.RequestServices.GetRequiredService<IAuthFilterProvider>();
            _responseAbstractFactory =
                _httpContext.RequestServices.GetRequiredService<IApiActionResponseAbstractFactory>();

            if (typeof(TRequest) == typeof(AbstractModel))
            {
                Data = (TRequest) (object) AbstractModel;
            }
            else
            {
                Data = initializationContext.HttpContext.RequestServices.GetRequiredService<IRequestModelFactory>()
                    .Create<TRequest>(initializationContext.AbstractModel);
            }

            // InitializeAsync Items
            return AppendedInitializeAsync(initializationContext, cancellationToken);
        }

        protected virtual Task AppendedInitializeAsync(IApiActionInitializationContext initializationContext,
            CancellationToken cancellationToken)
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

            var results = await Task.WhenAll(authorizationAttributes.Select(a =>
                a.AuthorizeAsync(_httpContext, AbstractModel, cancellationToken)));
            ActionResponse = ActionResponse ?? results.FirstOrDefault(r => r != null);
        }

        #endregion

        #region Data Pre-Load

        public virtual Task PreloadDataAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        [Response(400, typeof(BadRequestDetails), "Bad Request - Input Validation Failed")]
        public virtual Task<bool> ValidateModelAsync(CancellationToken cancellationToken)
        {
            if (typeof(TRequest) == typeof(AbstractModel))
            {
                return Task.FromResult(true);
            }

            var modelErrors = new List<ValidationResult>();
            if (Validator.TryValidateObject(Data, new ValidationContext(Data), modelErrors) && modelErrors.Count == 0)
            {
                return Task.FromResult(true);
            }

            Response(modelErrors);

            return Task.FromResult(false);
        }

        public virtual Task<bool> ValidateModelDataAsync(CancellationToken cancellationToken)
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

        protected ApiActionResponse Response()
        {
            return Response(NoContentResponse.Singleton);
        }

        protected ApiActionResponse Response(HttpStatusCode statusCode)
        {
            return Response((ApiActionResponse) new StatusCodeResponse(statusCode));
        }

        protected ApiActionResponse Response(int statusCode)
        {
            return Response((ApiActionResponse) new StatusCodeResponse(statusCode));
        }

        protected ApiActionResponse Response<T>(T data)
        {
            var response = _responseAbstractFactory.Create(typeof(T))?.Create(data, typeof(T));

            if (response == null)
            {
                throw new InvalidOperationException($"No responses could be generated for type {typeof(T)}. Please explicitly state inherited type using generic parameter or register custom IApiActionResponseFactory into dependency injection.");
            }

            return Response(response);
        }

        protected ApiActionResponse Response<T>(HttpStatusCode statusCode, T data)
        {
            return Response((int) statusCode, data);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected ApiActionResponse Response<T>(int statusCode, T data)
        {
            var response = _responseAbstractFactory.Create(typeof(T))?.Create(statusCode, data, typeof(T));

            if (response == null)
            {
                throw new InvalidOperationException(
                    $"No responses could be generated for type {typeof(T)}. Please explicitly state inherited type using generic parameter or register custom IApiActionResponseFactory into dependency injection.");
            }

            return Response(response);
        }

        protected ApiActionResponse Response(Stream data, string contentType)
        {
            return Response(new StreamResponse(data, contentType));
        }

        protected ApiActionResponse Response(int statusCode, Stream data, string contentType)
        {
            return Response(new StreamResponse(statusCode, data, contentType));
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected ApiActionResponse Response(ApiActionResponse response)
        {
            ActionResponse = response;
            return response;
        }

        #endregion
    }

    public abstract class ApiAction : ApiAction<AbstractModel>
    {
    }
}