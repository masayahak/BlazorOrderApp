namespace BlazorOrderApp.Repositories.Common
{
    public abstract class RepositoryBase
    {
        private readonly IHttpContextAccessor _contextAccessor;

        protected RepositoryBase(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected void CheckAuth()
        {
            return;

            //var user = _contextAccessor.HttpContext?.User;
            //if (user?.Identity?.IsAuthenticated != true)
            //{
            //    throw new UnauthorizedAccessException("認証が切れています。");
            //}
        }
    }
}
