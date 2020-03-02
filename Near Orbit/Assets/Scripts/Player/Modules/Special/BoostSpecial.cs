public class BoostSpecial : BaseSpecial {
    public override void Init(BaseShip owner) {
        Init(owner, 2.0f, 4.0f, true);
    }

    protected override void ApplyEffect() {
        owner.Movement.AmplifySpeed(1.5f);
    }
}
