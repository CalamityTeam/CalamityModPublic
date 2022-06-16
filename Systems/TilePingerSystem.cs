using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;
using System.Runtime.Serialization;
using Terraria.ID;
using Terraria.DataStructures;

namespace CalamityMod.Systems
{
    public class TilePingerSystem : ModSystem
    {
        internal static Dictionary<IPingedTileEffect, List<Point>> pingedTiles;
        internal static Dictionary<string, IPingedTileEffect> tileEffects;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            pingedTiles = new Dictionary<IPingedTileEffect, List<Point>>();
            tileEffects = new Dictionary<string, IPingedTileEffect>();
        }

        public override void Unload()
        {
            pingedTiles = null;
            tileEffects = null;
        }

        public static bool AddPing(string effectName, Vector2 position, Player pinger)
        {
            return AddPing(tileEffects[effectName], position, pinger);
        }

        public static bool AddPing(IPingedTileEffect effect, Vector2 position, Player pinger)
        {
            return effect.TryAddPing(position, pinger);
        }

        public static void RegisterTileToDraw(Point tilePos, string effectName)
        {
            RegisterTileToDraw(tilePos, tileEffects[effectName]);
        }

        public static void RegisterTileToDraw(Point tilePos, IPingedTileEffect effect)
        {
            if (!pingedTiles.ContainsKey(effect))
                pingedTiles.Add(effect, new List<Point>());

            pingedTiles[effect].Add(tilePos);
        }

        public override void PostUpdateEverything()
        {
            foreach (IPingedTileEffect effect in tileEffects.Values)
            {
                effect.UpdateEffect();
            }
        }

        public override void PostDrawTiles()
        {
            if (pingedTiles.Keys.Count < 1)
                return;

            foreach (IPingedTileEffect tileEffect in pingedTiles.Keys)
            {
                Effect effect = tileEffect.SetupEffect();

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, tileEffect.BlendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);

                foreach (Point tilePos in pingedTiles[tileEffect])
                {
                    tileEffect.PerTileSetup(tilePos, ref effect);
                    tileEffect.DrawTile(tilePos);

                }
                Main.spriteBatch.End();
            }

            pingedTiles.Clear();
        }
    }

    public class GlobalPingableTile : GlobalTile
    {
        public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            foreach(IPingedTileEffect effect in TilePingerSystem.tileEffects.Values)
            {
                if (effect.Active && effect.ShouldRegisterTile(i, j))
                    TilePingerSystem.RegisterTileToDraw(new Point(i, j), effect);
            }
        }
    }

    public interface IPingedTileEffect
    {
        public BlendState BlendState { get; }
        public Effect SetupEffect();
        public void PerTileSetup(Point pos, ref Effect effect);
        public void DrawTile(Point pos);
        public bool TryAddPing(Vector2 position, Player pinger);
        public bool Active { get; }
        public bool ShouldRegisterTile(int x, int y);
        public void UpdateEffect();
    }

    public class WulfrumPingTileEffect : IPingedTileEffect, ILoadable
    {
        internal static Texture2D emptyFrame;
        const int MaxPingLife = 280;
        const int MaxPingTravelTime = 140;
        const float PingWaveThickness = 40f;

        const float MaxPingRadius = 1400f;
        public static Vector2 PingCenter = Vector2.Zero;
        public static int PingTimer = 0;
        public static float PingProgress => (MaxPingLife - PingTimer) / (float)MaxPingLife;


        public bool Active => PingTimer > 0;

        public BlendState BlendState => BlendState.Additive;

        public Effect SetupEffect()
        {
            if (emptyFrame == null)
                emptyFrame = ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value;

            Effect tileEffect = Filters.Scene["WulfrumTilePing"].GetShader().Shader;

            tileEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
            tileEffect.Parameters["pingCenter"].SetValue(PingCenter);
            tileEffect.Parameters["pingRadius"].SetValue(400f);
            tileEffect.Parameters["pingWaveThickness"].SetValue(PingWaveThickness);
            tileEffect.Parameters["pingProgress"].SetValue(PingProgress);
            tileEffect.Parameters["pingTravelTime"].SetValue(MaxPingTravelTime / (float)MaxPingLife);
            tileEffect.Parameters["pingFadePoint"].SetValue(0.9f);
            tileEffect.Parameters["edgeBlendStrength"].SetValue(1f);
            tileEffect.Parameters["edgeBlendOutLenght"].SetValue(26f);
            tileEffect.Parameters["baseTintColor"].SetValue(Color.DeepSkyBlue.ToVector4() * 0.7f);
            tileEffect.Parameters["scanlineColor"].SetValue(Color.GreenYellow.ToVector4() * 0.9f);
            tileEffect.Parameters["tileEdgeColor"].SetValue(Color.GreenYellow.ToVector4());
            tileEffect.Parameters["waveColor"].SetValue(Color.GreenYellow.ToVector4());
            tileEffect.Parameters["Resolution"].SetValue(8f);

            return tileEffect;
        }

        public void PerTileSetup(Point pos, ref Effect effect)
        {
            //Top left, top right, bottom left, bottom right
            effect.Parameters["ordinalConnections"].SetValue(new Vector4(Connected(pos.X - 1, pos.Y - 1), Connected(pos.X + 1, pos.Y - 1), Connected(pos.X - 1, pos.Y + 1), Connected(pos.X + 1, pos.Y + 1)));
            //Up, left, right, down.
            effect.Parameters["cardinalConnections"].SetValue(new Vector4(Connected(pos.X, pos.Y - 1), Connected(pos.X - 1, pos.Y), Connected(pos.X + 1, pos.Y), Connected(pos.X, pos.Y + 1)));

            effect.Parameters["tilePosition"].SetValue(pos.ToVector2() * 16f);
        }

        public static float Connected(int x, int y) => Main.IsTileSpelunkable(x, y) ? 1f : 0f;

        public void DrawTile(Point pos)
        {
            Main.spriteBatch.Draw(emptyFrame, pos.ToWorldCoordinates() - Main.screenPosition, null, Color.White, 0, new Vector2(emptyFrame.Width / 2f, emptyFrame.Height / 2f), 16f, 0, 0);
        }

        public bool TryAddPing(Vector2 position, Player pinger)
        {
            //Only one ping at a time
            if (Active)
                return false;

            PingCenter = position;
            PingTimer = MaxPingLife;
            return true;
        }

        public bool ShouldRegisterTile(int x, int y)
        {
            return Main.IsTileSpelunkable(x, y);
        }

        public void UpdateEffect()
        {
            if (PingTimer > 0)
                PingTimer--;
        }

        public void Load(Mod mod)
        {
            TilePingerSystem.tileEffects.Add("WulfrumPing", this);
        }

        public void Unload()
        {

        }
    }
}
