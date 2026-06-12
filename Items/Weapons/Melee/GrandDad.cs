using System;
using System.Collections.Generic;
using CalamityMod.Dusts;
using CalamityMod.Items.BaseItems;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GrandDad : CustomUseProjItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static readonly SoundStyle GrandDadEasterEggSound = new("CalamityMod/Sounds/Custom/GFB/GrandDad");
        public Vector2 oldVel = Vector2.Zero;
        public int time = 0;
        public float highestSpeed = 0;
        public bool hitFloor = false;
        public override void SetDefaults()
        {
            Item.width = 124;
            Item.height = 124;
            Item.damage = 5777; // Feel free to change these 7s as balance requires. The other 7s should stay - Update: no more 2777... :(
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = 77;
            Item.useTime = 77;
            Item.useTurn = true;
            Item.knockBack = 77f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();

            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<GrandDadHoldout>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/GrandDadGlow").Value);
        }
        public override bool MeleePrefix() => true;

        // Has a GFB tooltip stuffed to the brim with various references
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            list.FindAndReplace("[GFB]", Lang.SupportGlyphs(this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal")));
        }
        public override void OnSpawn(IEntitySource source)
        {
            time = 0;
            hitFloor = false;
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Vector2 place = Item.Center + Vector2.UnitY * 48;
            if (time == 0 && Item.velocity.Y != 0)
            {
                oldVel = Item.velocity;
                if (!hitFloor)
                    Item.velocity = new Vector2(Math.Sign(Item.velocity.X) * 20, -10);
            }
            if (oldVel.Y != 0 && Item.velocity.Y == 0)
            {
                time = 0;
                float power = Utils.GetLerpValue(35, 75, highestSpeed, true) * 2;
                Main.LocalPlayer.SetScreenshake(3.5f * power);
                if (power > 0.25f)
                {
                    float blastSize = 150 * power;
                    float minMultiplier = 0.3f;
                    int hitsToMinMult = 8;
                    hitFloor = true;
                    Projectile blast = Projectile.NewProjectileDirect(Item.GetSource_FromThis(), place, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), (int)(Item.damage * 2 * power), -35 * power, -1, blastSize, minMultiplier, hitsToMinMult);
                    blast.timeLeft = 5;

                    int particleNumber = (int)Math.Max(15 * power, 2);
                    for (int i = -particleNumber; i <= particleNumber; i++)
                    {
                        if (i % 2 == 0)
                        {
                            Vector2 vel = (Vector2.UnitX * (5 + Math.Abs(i)) * Math.Sign(i)).RotatedByRandom(0.25f) * Main.rand.NextFloat(0.5f, 1) * power;
                            Dust dust2 = Dust.NewDustPerfect(place + vel * 13, ModContent.DustType<SquashDustPixelated>(),
                                vel, 0, default, Main.rand.NextFloat(1.3f, 1.2f));
                            dust2.noGravity = true;
                            dust2.color = Main.rand.NextBool() ? Color.Gold : Color.Goldenrod;
                            dust2.customData = new Vector2(0.2f, 1.6f);
                            dust2.fadeIn = 2.5f;
                        }
                        
                        Dust dust = Dust.NewDustPerfect(place, ModContent.DustType<VoidDustPixelated>(), (Vector2.UnitX * (5 + Math.Abs(i)) * Math.Sign(i)).RotatedByRandom(0.25f) * Main.rand.NextFloat(0.5f, 1) * power, 0, default, Main.rand.NextFloat(0.85f, 1.2f) * power);
                        dust.noGravity = true;
                        dust.color = Main.rand.NextBool() ? Color.Blue : Color.DodgerBlue;
                        dust.customData = 7;
                    }

                    SoundStyle sound = new("CalamityMod/Sounds/NPCHit/ExoHit3");
                    SoundEngine.PlaySound(sound with { Volume = 0.6f * power, Pitch = Main.rand.NextFloat(0.4f, 0.6f) }, Item.Center);
                    SoundStyle sound2 = new("CalamityMod/Sounds/NPCHit/ThanatosHitOpen1");
                    SoundEngine.PlaySound(sound2 with { Volume = 0.45f * power, Pitch = -0.2f }, Item.Center);
                }
                
                highestSpeed = 0;
            }
            if (Item.velocity.Y != 0)
            {
                if (Item.velocity.Y > 0 && Item.velocity.Y < 75)
                {
                    Item.velocity.X *= 0.98f;
                    Item.velocity.Y += 0.35f;
                    Item.velocity.Y *= 1.15f;
                }
                else
                    Item.velocity.Y += 0.55f;
                time++;
                if (highestSpeed < Item.velocity.Y)
                    highestSpeed = Item.velocity.Y;
            }

            if (Item.velocity.Y != 0)
                gravity = 0;
            maxFallSpeed = 75;
            oldVel = Item.velocity;
        }
        public override bool OnPickup(Player player)
        {
            if (Main.zenithWorld)
                SoundEngine.PlaySound(GrandDadEasterEggSound, Main.LocalPlayer.MountedCenter);
            return true;
        }
    }
}
