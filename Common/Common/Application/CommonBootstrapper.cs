using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Services.Common.Application.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Common.Application
{
    public class CommonBootstrapper
    {
        public static void Init(IServiceCollection service)
        {
            service.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));
        }
    }
}
