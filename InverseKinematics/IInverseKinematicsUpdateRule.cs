using Microsoft.Xna.Framework;

namespace CalamityMod.InverseKinematics
{
    public interface IInverseKinematicsUpdateRule
    {
        void Update(LimbCollection limbs, Vector2 destination);
    }
}
