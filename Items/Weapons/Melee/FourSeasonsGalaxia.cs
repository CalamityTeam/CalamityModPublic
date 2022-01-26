using CalamityMod.Items.Materials;
using CalamityMod.Buffs.Potions;
using CalamityMod.DataStructures;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader.IO;
using static CalamityMod.CalamityUtils;
using static CalamityMod.Items.Weapons.Melee.FourSeasonsGalaxia;
using static Terraria.ModLoader.ModContent;


namespace CalamityMod.Items.Weapons.Melee
{
    public class FourSeasonsGalaxia : ModItem
    {
        public Attunement mainAttunement = null;

        //Used for passive effects. On hit proc is never used but its just there so i can pass it as a reference in the passiveeffect function
        public int UseTimer = 0;
        public bool OnHitProc = false;

        #region stats
        public static int PhoenixAttunement_BaseDamage = 800;
        public static int PhoenixAttunement_LocalIFrames = 20; //Remember its got one extra update
        public static float PhoenixAttunement_BoltDamageReduction = 0.5f;
        public static float PhoenixAttunement_BoltThrowDamageMultiplier = 1.5f;
        public static float PhoenixAttunement_BaseDamageReduction = 0.2f;
        public static float PhoenixAttunement_FullChargeDamageBoost = 2f;

        public static int PolarisAttunement_BaseDamage = 1200;
        public static int PolarisAttunement_ShredIFrames = 10;
        public static int PolarisAttunement_LocalIFrames = 30; //Be warned its got one extra update so all the iframes should be divided in 2
        public static int PolarisAttunement_LocalIFramesCharged = 16;
        public static float PolarisAttunement_SlashDamageBoost = 5f; //Keep in mind the slice always crits
        public static int PolarisAttunement_SlashBoltsDamage = 1300;
        public static int PolarisAttunement_SlashIFrames = 60;
        public static float PolarisAttunement_ShotDamageBoost = 2f; //The shots fired if the dash connects

        public static int AndromedaAttunement_BaseDamage = 3500;
        public static int AndromedaAttunement_DashHitIFrames = 60;
        public static float AndromedaAttunement_FullChargeBoost = 1f; //The EXTRA damage boost. So putting 1 here will make it deal double damage. Putting 0.5 here will make it deal 1.5x the damage.
        public static float AndromedaAttunement_MonolithDamageBoost = 2f;
        public static float AndromedaAttunement_BoltsDamageReduction = 0.2f; //The shots fired as it charges

        public static int AriesAttunement_BaseDamage = 1420;
        public static int AriesAttunement_LocalIFrames = 10;
        public static int AriesAttunement_Reach = 600;
        public static float AriesAttunement_ChainDamageReduction = 0.2f;
        public static float AriesAttunement_OnHitBoltDamageReduction = 0.5f;

        public static int CancerPassiveDamage = 3000;
        public static int CancerPassiveLifeSteal = 3;
        public static float CancerPassiveLifeStealProc = 0.4f;
        public static int CapricornPassiveDebuffTime = 200;


        #endregion

        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxiaExtra"; //Just in case the player SOMEHOW gets to swing galaxia itself. Sprite also used as the base for other attacks

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxia");
            Tooltip.SetDefault("FUNCTION_DESC\n" +
                               "FUNCTION_EXTRA\n" +
                               "FUNCTION_PASSIVE\n" +
                               "Upgrading the sword let it break free from its earthly boundaries. You now have access to every single attunement at all times!\n" +
                               "Use RMB to cycle the sword's attunement forward or backwards depending on the position of your cursor\n" +
                               "Active Attunement : None\n" +
                               "Passive Blessing : None\n"); ;
        }

        #region tooltip editing

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;

            foreach (TooltipLine l in list)
            {
                if (l.text == null)
                    continue;

                if (l.text.StartsWith("FUNCTION_DESC"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipColor;
                        l.text = mainAttunement.function_description;
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Has 4 different functions that the owner can swap between";
                    }
                }

                if (l.text.StartsWith("FUNCTION_EXTRA"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipColor;
                        l.text = mainAttunement.function_description_extra;
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "This text should never be seen";
                    }
                }

                if (l.text.StartsWith("FUNCTION_PASSIVE"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipPassiveColor;
                        l.text = mainAttunement.passive_description;
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "This text should never be seen";
                    }
                }

                if (l.text.StartsWith("Active Attunement"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = Color.Lerp(mainAttunement.tooltipColor, mainAttunement.tooltipColor2, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.5f);
                        l.text = "Active Attumenent : [" + mainAttunement.name + "]";
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Active Attumenent : [None]";
                    }
                }

                if (l.text.StartsWith("Passive Blessing"))
                {
                    if (mainAttunement != null)
                    {
                        l.overrideColor = mainAttunement.tooltipPassiveColor;
                        l.text = "Passive Blessing : [" + mainAttunement.passive_name + "]";
                    }
                    else
                    {
                        l.overrideColor = new Color(163, 163, 163);
                        l.text = "Passive Blessing : [None]";
                    }
                }
            }
        }
        #endregion

        public override void SetDefaults()
        {
            item.width = item.height = 108;
            item.damage = 99;
            item.melee = true;
            item.useAnimation = 18;
            item.useTime = 18;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 24f;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<OmegaBiomeBlade>());
            recipe.AddIngredient(ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ItemType<DarksunFragment>(), 8);
            recipe.AddTile(TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        #region saving and syncing attunements
        public override bool CloneNewInstances => true;

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);
            if (Main.mouseItem.type == ItemType<FourSeasonsGalaxia>())
                item.modItem.HoldItem(Main.player[Main.myPlayer]);
            (clone as FourSeasonsGalaxia).mainAttunement = (item.modItem as FourSeasonsGalaxia).mainAttunement;

            return clone;
        }

        public override ModItem Clone() //ditto
        {
            var clone = base.Clone();
            (clone as FourSeasonsGalaxia).mainAttunement = mainAttunement;

            return clone;
        }

        public override TagCompound Save()
        {
            int attunement1 = mainAttunement == null ? -1 : (int)mainAttunement.id;
            TagCompound tag = new TagCompound
            {
                { "mainAttunement", attunement1 },
            };
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            int attunement1 = tag.GetInt("mainAttunement");

            mainAttunement = Attunement.attunementArray[attunement1 != -1 ? attunement1 : Attunement.attunementArray.Length - 1];
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(mainAttunement != null ? (byte)mainAttunement.id : Attunement.attunementArray.Length - 1);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            mainAttunement = Attunement.attunementArray[reader.ReadByte()];
        }

        #endregion

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (mainAttunement == null)
                return false;
            return true;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().rightClickListener = true;

            if (CanUseItem(player))
            {
                player.Calamity().LungingDown = false;
            }
            else
            {
                UseTimer++;
            }

            if (mainAttunement == null)
                mainAttunement = Attunement.attunementArray[(int)AttunementID.Phoenix];

            mainAttunement.ApplyStats(item);

            //Passive effects only jappen player side haha
            if (player.whoAmI != Main.myPlayer)
                return;

            mainAttunement.PassiveEffect(player, ref UseTimer, ref OnHitProc);

            if (player.Calamity().mouseRight && CanUseItem(player) && player.whoAmI == Main.myPlayer && !Main.mapFullscreen)
            {
                //Don't shoot out a visual blade if you already have one out
                if (Main.projectile.Any(n => n.active && n.type == ProjectileType<GalaxiaHoldout>() && n.owner == player.whoAmI))
                    return;

                Projectile.NewProjectile(player.Top, Vector2.Zero, ProjectileType<GalaxiaHoldout>(), 0, 0, player.whoAmI, 0, Math.Sign(player.position.X - Main.MouseWorld.X));
            }
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI &&
            (n.type == ProjectileType<PhoenixsPride>() ||
             n.type == ProjectileType<AndromedasStride>() ||
             n.type == ProjectileType<PolarissGaze>() ||
             n.type == ProjectileType<AriessWrath>()
            ));
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (mainAttunement == null)
                mainAttunement = new PhoenixAttunement();

            Texture2D itemTexture = GetTexture((mainAttunement.id == AttunementID.Polaris || mainAttunement.id == AttunementID.Andromeda) ? "CalamityMod/Items/Weapons/Melee/GalaxiaRed" : "CalamityMod/Items/Weapons/Melee/GalaxiaBlue");
            spriteBatch.Draw(itemTexture, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D itemTexture = GetTexture((mainAttunement.id == AttunementID.Polaris || mainAttunement.id == AttunementID.Andromeda) ? "CalamityMod/Items/Weapons/Melee/GalaxiaRed" : "CalamityMod/Items/Weapons/Melee/GalaxiaBlue");
            spriteBatch.Draw(itemTexture, item.Center - Main.screenPosition, null, lightColor, rotation, item.Size * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class GalaxiaHoldout : ModProjectile //Visuals. Now much simpler than before!
    {
        private Player Owner => Main.player[projectile.owner];
        public bool OwnerCanUseItem => Owner.HeldItem == associatedItem ? (Owner.HeldItem.modItem as FourSeasonsGalaxia).CanUseItem(Owner) : false;
        public ref float Initialized => ref projectile.ai[0];
        public ref float CycleDirection => ref projectile.ai[1];

        private Item associatedItem;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galactic Formation");
        }
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 2;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 40;
            projectile.tileCollide = false;
            projectile.damage = 0;
        }

        public override void AI()
        {

            if (Initialized == 0f)
            {
                //If dropped, kill it instantly
                if (Owner.HeldItem.type != ItemType<FourSeasonsGalaxia>())
                {
                    projectile.Kill();
                    return;
                }

                projectile.Center = Owner.Center;
                associatedItem = Owner.HeldItem;

                //Switch up the attunements
                Reattune((FourSeasonsGalaxia)associatedItem.modItem);

                //Do particles around the player 

                Color particleColor = (associatedItem.modItem as FourSeasonsGalaxia).mainAttunement.tooltipColor;
                for (int i = 0; i <= 5; i++)
                {
                    Vector2 displace = Vector2.UnitX * 20 * Main.rand.NextFloat(-1f, 1f);
                    Particle Glow = new GenericBloom(Owner.Bottom + displace, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f), particleColor, 0.02f + Main.rand.NextFloat(0f, 0.2f), 20 + Main.rand.Next(30));
                    GeneralParticleHandler.SpawnParticle(Glow);
                }
                for (int i = 0; i <= 10; i++)
                {
                    Vector2 displace = Vector2.UnitX * 16 * Main.rand.NextFloat(-1f, 1f);
                    Particle Sparkle = new GenericSparkle(Owner.Bottom + displace, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f), particleColor, particleColor, 0.5f + Main.rand.NextFloat(-0.2f, 0.2f), 20 + Main.rand.Next(30), 1, 2f);
                    GeneralParticleHandler.SpawnParticle(Sparkle);
                }

                Initialized = 1f;
            }
        }

        public void Reattune(FourSeasonsGalaxia item)
        {
            Attunement attunement;
            Vector2[] StarPositions;
            Vector2[] ExtraLines; //Extra lines vector give us an indication of the index between the 2 stars we need to connect
            List<int> IgnoredLines = new List<int>();

            Particle Star;
            Particle Line;
            Color StarColor;

            if (CycleDirection == -1) //Cycles goes Phoenix => Aries => Polaris => Andromeda
            {
                switch (item.mainAttunement.id)
                {
                    case AttunementID.Phoenix: //Switching to the aries attunement. 
                        attunement = Attunement.attunementArray[(int)AttunementID.Aries];
                        break;
                    case AttunementID.Aries: //Switching to the polaris attunement. 
                        attunement = Attunement.attunementArray[(int)AttunementID.Polaris];
                        break;
                    case AttunementID.Polaris: //Switching to the andromeda attunement. 
                        attunement = Attunement.attunementArray[(int)AttunementID.Andromeda];
                        break;
                    case AttunementID.Andromeda: //Switching to the phoenix attunement. 
                    default:
                        attunement = Attunement.attunementArray[(int)AttunementID.Phoenix];
                        break;
                }
            }
            else //Cycles goes Phoenix <= Aries <= Polaris <= Andromeda
            {
                switch (item.mainAttunement.id)
                {
                    case AttunementID.Phoenix: //Switching to the andromeda attunement. 
                        attunement = Attunement.attunementArray[(int)AttunementID.Andromeda];
                        break;
                    case AttunementID.Andromeda: //Switching to the polaris attunement. 
                        attunement = Attunement.attunementArray[(int)AttunementID.Polaris];
                        break;
                    case AttunementID.Polaris: //Switching to the aries attunement. 
                        attunement = Attunement.attunementArray[(int)AttunementID.Aries];
                        break;
                    case AttunementID.Aries: //Switching to the phoenix attunement. 
                    default:
                        attunement = Attunement.attunementArray[(int)AttunementID.Phoenix];
                        break;
                }
            }

            switch (attunement.id)
            {
                case AttunementID.Aries: // Drawing Aries
                    StarPositions = new Vector2[] { new Vector2(-160, -150), new Vector2(45, -170), new Vector2(137, 40), new Vector2(146, 126), new Vector2(129, 151) };
                    ExtraLines = new Vector2[] { };
                    StarColor = Color.Orchid;
                    break;
                case AttunementID.Polaris: //Drawing Ursa Minor
                    StarPositions = new Vector2[] { new Vector2(69, -188), new Vector2(18, -122), new Vector2(-23, -39), new Vector2(-13, 63), new Vector2(42, 147), new Vector2(-8, 184), new Vector2(-61, 83) };
                    ExtraLines = new Vector2[] { new Vector2(3, 6) };
                    StarColor = Color.CornflowerBlue;
                    break;
                case AttunementID.Andromeda: //Drawing Andromeda
                    //https://media.discordapp.net/attachments/802291445360623686/934200685254291546/unknown.png
                    StarPositions = new Vector2[] { 
                        new Vector2(-210, -46), new Vector2(-150, -35), new Vector2(-69, 18), new Vector2(33, 61), new Vector2(127, 72), //The horizontal line
                        new Vector2(-41, -27), new Vector2(-41, -67), new Vector2(-100, -124), new Vector2(-160, -130), //The first branch from the left
                        new Vector2(15, 147), new Vector2(35, 126), new Vector2(37, 23), new Vector2(67, -47), new Vector2(126, -109), new Vector2(146, -136), //The second branch from the left
                        new Vector2(95, -117)
                    };
                    ExtraLines = new Vector2[] { new Vector2(2, 5) , new Vector2(13, 15) };
                    IgnoredLines.Add(5);
                    IgnoredLines.Add(9);
                    IgnoredLines.Add(15);
                    StarColor = Color.MediumSlateBlue;
                    break;
                case AttunementID.Phoenix: //Drawing Phoenix
                default: 
                    StarPositions = new Vector2[] { new Vector2(-206, -99), new Vector2(-150, -43), new Vector2(-120, -146), new Vector2(-60, -71), new Vector2(-106, 71), new Vector2(-59, 138), new Vector2(116, -22),//The main line
                    new Vector2(246, -26),  new Vector2(192, 36), new Vector2(138, 81), //The side bit
                    new Vector2(88, -107), new Vector2(84, -75) }; //Complete the loop
                    ExtraLines = new Vector2[] { new Vector2(3, 10), new Vector2(11, 6) };
                    IgnoredLines.Add(10);
                    StarColor = Color.OrangeRed;
                    break;
            }

            //Only draw the cool effect if you have enough particle slots left. Itd look really ugly to have a half complete constellation
            if (GeneralParticleHandler.FreeSpacesAvailable() > StarPositions.Length * 2)
            {
                for (int i = 0; i < StarPositions.Length; i++)
                {
                    //The polar star gets bigger than the others. Also since andromeda has so many stars, make em smaller
                    Star = new GenericSparkle(Owner.Center + StarPositions[i], Vector2.Zero, Color.White, StarColor, (attunement.id == AttunementID.Polaris && i == 0) ? 3f : Main.rand.NextFloat(1f, 1.5f) * (attunement.id == AttunementID.Andromeda ? 0.8f : 1f), 20, 0f, 3f);
                    GeneralParticleHandler.SpawnParticle(Star);

                    if (i > 0 && !IgnoredLines.Contains(i))
                    {
                        Line = new BloomLineVFX(Owner.Center + StarPositions[i - 1], StarPositions[i] - StarPositions[i - 1], 0.5f, StarColor, 20, true);
                        GeneralParticleHandler.SpawnParticle(Line);
                    }
                }

                for (int i = 0; i < ExtraLines.Length; i++)
                {
                    Line = new BloomLineVFX(Owner.Center + StarPositions[(int)ExtraLines[i].Y], StarPositions[(int)ExtraLines[i].X] - StarPositions[(int)ExtraLines[i].Y], 0.5f, StarColor, 20, true);
                    GeneralParticleHandler.SpawnParticle(Line);
                }
            }

            //Chunger
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ThunderStrike"), projectile.Center);
            if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 5)
                Main.LocalPlayer.Calamity().GeneralScreenShakePower = 5;
            item.mainAttunement = attunement;
        } 
    }
}
