using FluentValidation;

namespace SimpleWebApiAuth.API.Validation
{
    public class ValidationFilter<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
        {
            var validator = ctx.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validator is null)
                return await next(ctx);

            var entity = ctx.Arguments.OfType<T>().FirstOrDefault(a => a?.GetType() == typeof(T));

            if (entity is null)
                return TypedResults.Problem("Could not find type to validate");

            var validation = await validator.ValidateAsync(entity);

            if (!validation.IsValid)
                return TypedResults.ValidationProblem(validation.ToDictionary());

            return await next(ctx);
        }
    }
}
