namespace FuelSDK
{
    internal interface IAuthService
    {
        void RefreshToken(bool force = false);
    }
}