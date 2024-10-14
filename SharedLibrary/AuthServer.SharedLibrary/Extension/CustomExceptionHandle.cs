using AuthServer.SharedLibrary.Dtos;
using AuthServer.SharedLibrary.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AuthServer.SharedLibrary.Extension
{
    public static class CustomExceptionHandle
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorFeatures = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorFeatures != null) 
                    {
                        var ex = errorFeatures.Error;
                        ErrorDto errDto = null;

                        if (ex is CustomException) 
                        {
                           errDto = new ErrorDto(ex.Message,true);
                        }
                        else 
                        {
                           errDto = new ErrorDto(ex.Message, false);
                        }


                        var response = Response<NoDataDto>.Fail(errDto, 500);
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                });
            });
        }
    }
}
