public class GuestOrAuthExclusive : AuthExclusive {
    protected override bool IsAllowed(AuthManager auth) {
        return auth.IsAuthenticated || auth.IsGuestMode;
    }
}
