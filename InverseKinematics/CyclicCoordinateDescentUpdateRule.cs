using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace CalamityMod.InverseKinematics
{
    public class CyclicCoordinateDescentUpdateRule : IInverseKinematicsUpdateRule
    {
        public float AngularOffsetAcceleration;
        public float AngularDeviationLenience;
        public CyclicCoordinateDescentUpdateRule(float angularOffsetAcceleration, float angularDeviationLenience)
        {
            AngularOffsetAcceleration = angularOffsetAcceleration;
            AngularDeviationLenience = angularDeviationLenience;
        }

        public void Update(LimbCollection limbs, Vector2 destination)
        {
            float distanceFromStart = Vector2.Distance(destination, limbs.ConnectPoint);
            float distanceFromEnd = Vector2.Distance(destination, limbs.EndPoint);
            float slowdownInterpolant = Utils.GetLerpValue(8f, 40f, distanceFromEnd, true);
            slowdownInterpolant *= Utils.GetLerpValue(8f, 40f, distanceFromStart, true);

            Vector2 originalEndPoint = limbs.EndPoint;
            for (int i = limbs.Limbs.Length - 1; i >= 0; i--)
            {
                // Move based on angular offsets. Movement is dampened the closer a limb is to being the first limb.
                Vector2 currentToEndOffset = originalEndPoint - limbs.Limbs[i].ConnectPoint;
                Vector2 currentToDestinationOffset = destination - limbs.Limbs[i].ConnectPoint;
                Vector2 perpendicularDirection = currentToDestinationOffset.RotatedBy(MathHelper.PiOver2);
                float angularOffset = currentToEndOffset.AngleBetween(currentToDestinationOffset) * (float)Math.Sqrt((i + 1f) / limbs.Limbs.Length);

                // Determine direction by choosing the angle which approaches the destination faster.
                float leftAngularOffset = currentToEndOffset.AngleBetween(currentToDestinationOffset - perpendicularDirection);
                float rightAngularOffset = currentToEndOffset.AngleBetween(currentToDestinationOffset + perpendicularDirection);
                if (leftAngularOffset > rightAngularOffset)
                    angularOffset *= -1f;

                // Perform safety checks on the result of the underlying arccosines.
                if (float.IsNaN(angularOffset))
                    break;

                // Update rotation.
                limbs.Limbs[i].Rotation += angularOffset * slowdownInterpolant * AngularOffsetAcceleration;

                // And limit it so that it doesn't look weird.
                if (i > 0)
                {
                    float behindRotation = (float)limbs.Limbs[i - 1].Rotation;
                    limbs.Limbs[i].Rotation = MathHelper.Clamp((float)limbs.Limbs[i].Rotation, behindRotation - AngularDeviationLenience, behindRotation + AngularDeviationLenience);
                }
            }
        }
    }
}
