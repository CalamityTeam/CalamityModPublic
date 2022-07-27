using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.InverseKinematics
{
    public class Limb
    {
        // Doubles are used instead of floats as a means of providing sufficient precision to not cause erroneous results when
        // doing approximations of derivative limits with small divisors.
        public double Rotation;
        public double Length;
        public Vector2 ConnectPoint;
        public Vector2 EndPoint => ConnectPoint + ((float)Rotation).ToRotationVector2() * (float)Length;

        public Limb(float rotation, float length)
        {
            Rotation = rotation;
            Length = length;
        }
    }
}
