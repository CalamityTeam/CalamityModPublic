using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Fishing
{
    public class Spadefish : RogueWeapon, ILocalizedModType
    {
        public static float SpinsToThrow => 3;

        public static float SpinDamageMult => 0.33f;

        public static float SpinAccel => 1.0175f;

        public static float SpinThrowVelocityMult => 1.5f;

        public override float StealthDamageMultiplier => 2f;

        public static int PickPower => 50;
        public new string LocalizationCategory => "Items.Fishing";
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 44;
            Item.damage = 30;
            Item.knockBack = 2f;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.shootSpeed = 12;
            Item.DamageType = RogueDamageClass.Instance;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SpadefishThrown>();
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.pick = PickPower;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                type = ModContent.ProjectileType<SpadefishHoldout>();
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI); //This proj sets StealthStrike on its own setdefaults

                player.Calamity().ConsumeStealthByAttacking(); //Due to the holdout, we manually consume stealth here and on spam.
                return false;
            }
            player.Calamity().rogueStealth = 0;
            return true;
        }


        #region Toggleable Mining

        public bool CanMine = false;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplace("[TOGGLE]", CanMine ? this.GetLocalizedValue("TextWhileEnabled") : this.GetLocalizedValue("TextWhileDisabled"));
        }
        public override bool CanRightClick() => Main.keyState.PressingShift();
        public override void RightClick(Player player)
        {
            CanMine = !CanMine;
            Item.NetStateChanged();
        }
        public override bool ConsumeItem(Player player) => false;
        public override void SaveData(TagCompound tag)
        {
            tag.Add("CanMine", CanMine);
        }

        public override void LoadData(TagCompound tag)
        {
            CanMine = tag.GetBool("CanMine");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(CanMine);
        }

        public override void NetReceive(BinaryReader reader)
        {
            CanMine = reader.ReadBoolean();
        }
        Asset<Texture2D> texture;
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (CanMine)
            {
                var tex = CalamityUtils.GetTextureEfficient(ref texture, "Terraria/Images/Extra_48").Value;
                var dotFrame = tex.Frame(8, 39, 4, 23);
                spriteBatch.Draw(tex, position + new Vector2(16, 16) * Main.inventoryScale, dotFrame, Color.White, 0, dotFrame.Size() * 0.5f, Main.inventoryScale, SpriteEffects.None, 0);
            }
        }
        #endregion
    }

    public class SpadefishThrown : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Fishing/Spadefish";
        public override void SetDefaults()
        {
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 3;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.timeLeft = 480;
            Projectile.MaxUpdates = 2;
            Projectile.ignoreWater = true;
        }
        bool frameOne = true;
        public override void AI()
        {
            if (frameOne)
            {
                Projectile.velocity /= Projectile.MaxUpdates;
                frameOne = false;
            }
            if (Projectile.ai[0] <= 1 && Projectile.timeLeft < 450)
                Projectile.velocity.Y += 0.08f;
            if (Projectile.ai[0] == 0)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else if (Projectile.ai[0] == 1)
                Projectile.rotation += Projectile.velocity.X * 0.15f;

            if (Projectile.timeLeft < 60)
            {
                Projectile.Opacity = Projectile.timeLeft / 60f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            if (Projectile.ai[0] == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.velocity = Vector2.Zero;
            Projectile.Center += oldVelocity;
            Projectile.ai[0] = 2;
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            if (Main.myPlayer == Projectile.owner && Main.LocalPlayer.HeldItem.ModItem is Spadefish s && s.CanMine)
                for (var j = -1; j < 2; j++)
                    for (var k = -1; k < 2; k++)
                        Dig(Spadefish.PickPower, Projectile.Center.ToTileCoordinates() + new Point(j, k));
            return false;
        }

        public static void Dig(int pickPower, Point point)
        {
            var tile = Main.tile[point];
            var x = tile.X();
            var y = tile.Y();

            if (!tile.HasTile || Main.tileAxe[tile.TileType] || Main.tileHammer[tile.TileType])
                return;

            int hitIndex = Main.LocalPlayer.hitTile.HitObject(x, y, 1);

            if (tile.TileType == TileID.MysticSnakeRope)
                return;

            int pickDmg = Main.LocalPlayer.GetPickaxeDamage(x, y, pickPower, hitIndex, tile);


            if (!WorldGen.CanKillTile(x, y))
                pickDmg = 0;

            if (Main.getGoodWorld)
                pickDmg *= 2;

            if (Main.LocalPlayer.DoesPickTargetTransformOnKill(Main.LocalPlayer.hitTile, pickDmg, x, y, pickPower, hitIndex, tile))
                pickDmg = 0;

            if (Main.LocalPlayer.hitTile.AddDamage(hitIndex, pickDmg) >= 100)
            {
                AchievementsHelper.CurrentlyMining = true;
                Main.LocalPlayer.ClearMiningCacheAt(x, y, 1);
                if (Main.netMode == NetmodeID.MultiplayerClient && Main.tileContainer[tile.TileType])
                {
                    if (tile.TileType == TileID.DisplayDoll || tile.TileType == TileID.HatRack)
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 20, x, y);
                    else
                    {
                        WorldGen.KillTile(x, y, fail: true);
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y, 1f);
                    }
                    if (tile.TileType == TileID.Containers)
                    {
                        NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 1, x, y);
                    }
                    if (tile.TileType == TileID.Containers2)
                    {
                        NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 5, x, y);
                    }
                    if (tile.TileType == TileID.Dressers)
                    {
                        NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 3, x, y);
                    }
                }
                else
                {
                    bool flag = tile.HasTile;
                    WorldGen.KillTile(x, y);
                    if (!Main.dedServ && flag && !tile.HasTile)
                    {
                        AchievementsHelper.HandleMining();
                    }
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
                    }
                }
                AchievementsHelper.CurrentlyMining = false;
            }
            else
            {
                WorldGen.KillTile(x, y, fail: true);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y, 1f);
                    NetMessage.SendData(MessageID.SyncTilePicking, -1, -1, null, Main.myPlayer, x, y, pickDmg);
                }
            }
            if (pickDmg != 0)
            {
                Main.LocalPlayer.hitTile.Prune();
            }
        }




        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item178 with { Pitch = -0.5f + 0.25f * Projectile.numHits + (Projectile.Calamity().stealthStrike ? 1f : 0f) }, Projectile.Center);

            Projectile.velocity.X = -Projectile.velocity.X.DirectionalSign();
            Projectile.velocity.Y = -2f;
            Projectile.ai[0] = 1;
            Projectile.damage /= 2;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var tex = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(tex.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, 0);
            return false;
        }
    }

    public class SpadefishHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Fishing/Spadefish";

        ref float spin => ref Projectile.ai[0];
        ref float startingDir => ref Projectile.ai[1];

        float throwSpeed = 12; //changing this does nothing, as this is set in the code. This is just a fallback value if something breaks.

        bool reset = false;

        float oldSpin = 0;
        int digDir = 0;

        float spinSin => MathF.Sin(spin);
        public override void SetDefaults()
        {
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.Calamity().stealthStrike = true;
            Projectile.tileCollide = false;
        }

        public override void Load()
        {
            On_LegacyPlayerRenderer.DrawPlayer += spinThePlayer;
        }

        private void spinThePlayer(On_LegacyPlayerRenderer.orig_DrawPlayer orig, LegacyPlayerRenderer self, Terraria.Graphics.Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow, float scale)
        {
            var t = ModContent.ProjectileType<SpadefishHoldout>();
            if (drawPlayer.ownedProjectileCounts[t] <= 0)
            {
                orig(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, scale);
                return;
            }

            var Spade = Main.projectile.FirstOrDefault(p => p.active && p.owner == drawPlayer.whoAmI && p.type == t)?.ModProjectile<SpadefishHoldout>();

            if (Spade is null || Spade.startingDir == 0)
            {
                orig(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, scale);
                return;
            }

            using var lease = ScreenspaceTargetPool.Shared.Rent(Main.instance.GraphicsDevice);
            using (lease.Scope(clearColor: Color.Transparent))
            {
                orig(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, scale);

                bool flip = (Spade.spinSin * Spade.startingDir) < 0;
                var tex = TextureAssets.Projectile[Type];
                Main.EntitySpriteDraw(tex.Value, drawPlayer.Center + new Vector2(40, 0) - Main.screenPosition, null, Lighting.GetColor(Spade.Projectile.Center.ToTileCoordinates()), Spade.Projectile.rotation - (flip ? MathHelper.PiOver2 : 0), tex.Size() * 0.5f, new Vector2(Spade.Projectile.scale, Spade.Projectile.scale), flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            }
            float width = Spade.spinSin * Spade.startingDir * drawPlayer.direction;

            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.identity);
                Main.spriteBatch.Draw(lease.Target, position - Main.screenPosition, null, Color.White, 0, position - Main.screenPosition, new Vector2(MathF.Abs(width), 1), width < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                Main.spriteBatch.End();
            }
        }

        public override void AI()
        {
            var player = Main.player[Projectile.owner];
            if (startingDir == 0)
            {
                startingDir = Projectile.velocity.X.DirectionalSign();
                throwSpeed = Projectile.velocity.Length();
                Projectile.velocity = Vector2.Zero;
            }
            player.direction = 1;
            player.SetDummyItemTime((int)(player.HeldItem.useTime * player.GetWeaponAttackSpeed(player.HeldItem)));
            Projectile.Center = player.Center + new Vector2(48 * spinSin, 0 * MathF.Cos(spin)) * startingDir;
            Projectile.rotation = Projectile.DirectionFrom(player.Center).ToRotation() + MathHelper.PiOver4;
            if (spin == 0)
            {
                spin = MathHelper.TwoPi;
            }
            spin *= Spadefish.SpinAccel;

            int res = reset ? -1 : 1;
            if (res != spinSin.DirectionalSign())
            {
                Projectile.ResetLocalNPCHitImmunity();
                reset = !reset;
            }

            if (Projectile.timeLeft % 5 == 0 && Main.myPlayer == Projectile.owner && Main.LocalPlayer.HeldItem.ModItem is Spadefish s && s.CanMine)
                for (var j = -1; j < 2; j++)
                    for (var k = -1; k < 2; k++)
                        SpadefishThrown.Dig(Spadefish.PickPower, Projectile.Center.ToTileCoordinates() + new Point(j, k));

            if (spin > MathHelper.TwoPi * (1 + Spadefish.SpinsToThrow))
            {
                player.direction = (int)startingDir;
                if (Main.myPlayer == Projectile.owner)
                {
                    var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, player.Calamity().mouseRotationFromPlayer.ToRotationVector2() * throwSpeed * Spadefish.SpinThrowVelocityMult, ModContent.ProjectileType<SpadefishThrown>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                    p.Calamity().stealthStrike = true;
                }
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item178 with { pitch = (spin-MathHelper.TwoPi) / (MathHelper.TwoPi * Spadefish.SpinsToThrow) - 0.5f }, Projectile.Center);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= Spadefish.SpinDamageMult;
        }
    }
}
