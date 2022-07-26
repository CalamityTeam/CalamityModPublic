using Microsoft.Xna.Framework;
using System.Linq;

namespace CalamityMod.InverseKinematics
{
    public class LimbCollection
    {
        public Limb[] Limbs;
        public IInverseKinematicsUpdateRule UpdateRule;
        public Vector2 ConnectPoint => Limbs.First().ConnectPoint;
        public Vector2 EndPoint => Limbs.Last().EndPoint;
        public double TotalLength => Limbs.Sum(l => l.Length);

        public LimbCollection(IInverseKinematicsUpdateRule updateRule, int limbCount, float limbLength)
        {
            UpdateRule = updateRule;

            Limbs = new Limb[limbCount];
            for (int i = 0; i < limbCount; i++)
                Limbs[i] = new Limb(0f, limbLength);
        }

        public LimbCollection(IInverseKinematicsUpdateRule updateRule, params float[] limbLengths)
        {
            UpdateRule = updateRule;

            int limbCount = limbLengths.Length;
            Limbs = new Limb[limbCount];
            for (int i = 0; i < limbCount; i++)
                Limbs[i] = new Limb(0f, limbLengths[i]);
        }

        public void UpdateConnectPoints(Vector2? connectPoint = null)
        {
            if (connectPoint.HasValue)
                Limbs[0].ConnectPoint = connectPoint.Value;

            for (int i = 1; i < Limbs.Length; i++)
                Limbs[i].ConnectPoint = Limbs[i - 1].EndPoint;
        }

        public Limb this[int index]
        {
            get => Limbs[index];
            set => Limbs[index] = value;
        }

        public void Update(Vector2 connectPoint, Vector2 destination)
        {
            UpdateRule.Update(this, destination);
            Limbs[0].ConnectPoint = connectPoint;
            UpdateConnectPoints(connectPoint);
        }
    }
}
