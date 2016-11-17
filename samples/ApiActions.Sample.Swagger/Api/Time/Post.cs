using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DanielAHill.AspNetCore.ApiActions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiActions.Sample.Swagger.Api.Time
{
    [Post]
    [Summary("Displays information about requested time zone")]
    [DanielAHill.AspNetCore.ApiActions.Description("Demonstrates how to validate request data, following basic validation of format by overriding the ValidateModelDataAsync method.")]
    [DanielAHill.AspNetCore.ApiActions.Category(SwaggerCategories.Validation)]
    public class Post : ApiAction<Post.RequestModel>
    {
        private TimeZoneInfo _timeZone;

        public override Task<bool> ValidateModelDataAsync(CancellationToken cancellationToken)
        {
            _timeZone = TimeZoneInfo.GetSystemTimeZones()
                    .FirstOrDefault(tz => tz.Id.Equals(Data.TimeZone, StringComparison.OrdinalIgnoreCase)
                                          || tz.StandardName.Equals(Data.TimeZone, StringComparison.OrdinalIgnoreCase)
                                          || tz.DaylightName.Equals(Data.TimeZone, StringComparison.OrdinalIgnoreCase)
                                          || tz.BaseUtcOffset.ToString().Equals(Data.TimeZone, StringComparison.OrdinalIgnoreCase));

            if (_timeZone == null)
            {
                Response(new ValidationResult[]
                {
                    new ValidationResult($"Time Zone Not Found. Try: {TimeZoneInfo.Local.Id}",
                        new[] {"TimeZone"})
                });

                return Task.FromResult(false);
            }

            return base.ValidateModelDataAsync(cancellationToken);
        }

        [Response(200, typeof(ResponseModel))]
        public override Task<ApiActionResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Response(new ResponseModel()
            {
                CurrentUtc = DateTime.UtcNow,
                Current = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _timeZone),
                TimeZoneInfo = _timeZone
            });
        }

        public class RequestModel
        {
            [Required]
            public string TimeZone { get; set; }
        }

        public class ResponseModel
        {
            public DateTime Current { get; set; }
            public DateTime CurrentUtc { get; set; }
            public TimeZoneInfo TimeZoneInfo { get; set; }
        }
    }
}
