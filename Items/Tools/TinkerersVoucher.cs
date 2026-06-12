using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Prefixes;
using CalamityMod.Projectiles.Environment;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public static class VoucherReforgeSystem
    {
        public static int ForcedPrefix = -1;
        public static bool RollWon = false;
    }

    public class VoucherGlobalItem : GlobalItem
    {
        public override void PreReforge(Item item)
        {
            Player player = Main.LocalPlayer;
            bool isAccessory = item.accessory;

            VoucherReforgeSystem.ForcedPrefix = -1;

            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item invItem = player.inventory[i];

                if (invItem.ModItem is VoucherItem voucher)
                {
                    if (voucher.IsUsableVoucher(player, item, isAccessory))
                    {
                        VoucherReforgeSystem.ForcedPrefix = voucher.GetPrefixForItem(player, item, isAccessory);

                        invItem.stack--;
                        if (invItem.stack <= 0)
                            invItem.TurnToAir();

                        if (Main.netMode != NetmodeID.Server)
                        {
                            if (VoucherReforgeSystem.RollWon)
                            {
                                for (int dustCount = 0; dustCount < 8; dustCount++)
                                {
                                    // Light dust and sparks
                                    Vector2 sparkVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3f, 12f);
                                    Vector2 dustVel = (sparkVel).SafeNormalize(Vector2.UnitY).RotatedBy(Main.rand.Next(100)) * Main.rand.NextFloat(1f, 15f);
                                    Vector2 fxPos = player.Center + sparkVel;
                                    Color fxColor = Color.Lerp(Color.MediumAquamarine, Color.MediumSeaGreen, Main.rand.NextFloat(1f));

                                    Particle fx = new CustomSpark(fxPos, sparkVel, "CalamityMod/Particles/Sparkle", false, (int)(Main.rand.Next(30, 60)), Main.rand.NextFloat(1.3f, 1.8f), fxColor, new Vector2(0.5f, 1.1f), extraRotation: 0, shrinkSpeed: Main.rand.NextFloat(0.1f, 0.3f));
                                    GeneralParticleHandler.SpawnParticle(fx);

                                    Dust dust = Dust.NewDustPerfect(fxPos, ModContent.DustType<LightDust>(), dustVel, 0, default, Main.rand.NextFloat(0.8f, 1.6f));
                                    dust.noGravity = true;
                                    dust.color = fxColor;


                                    // Crit sparkles
                                    Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1f, 25f);
                                    Color color = Main.rand.NextBool() ? Color.LightBlue : Color.LightSkyBlue;

                                    Particle sparkle = new CritSpark(player.MountedCenter, velocity, color, bloom: Color.Cyan, scale: 1f, lifeTime: Main.rand.Next(15, 60));
                                    GeneralParticleHandler.SpawnParticle(sparkle);
                                }

                                SoundEngine.PlaySound(SoundID.Item4, player.position);
                            }
                            else
                            {
                                for (int d = 0; d < 10; d++)
                                {
                                    for (int smokeCount = 0; smokeCount < 3; smokeCount++)
                                    {
                                        Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3f, 12f);
                                        Color smokeStart = Main.rand.NextBool() ? Color.Gray : Color.LightGray;
                                        Color smokeEnd = Color.DimGray;
                                        float smokeSize = Main.rand.NextFloat(0.9f, 2f);

                                        Particle smoke = new SmallSmokeParticle(player.MountedCenter, velocity, smokeStart, smokeEnd, smokeSize, Main.rand.Next(90, 140));
                                        GeneralParticleHandler.SpawnParticle(smoke);
                                    }

                                    Particle skull = new DesertProwlerSkullParticle(player.Center, new Vector2(2.5f, 2.5f).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f), Main.rand.NextBool() ? Color.LightGray : Color.Silver, Color.Gray, Main.rand.NextFloat(0.15f, 0.5f), Main.rand.Next(100, 190));
                                    GeneralParticleHandler.SpawnParticle(skull);           
                                }

                                SoundEngine.PlaySound(SoundID.Item16, player.position);
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    public abstract class VoucherItem : ModItem
    {
        #region CyclePrefixes
        protected virtual int[] prefixChoices => Array.Empty<int>();
        protected int chosenPrefix = 0;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string selected = GetSelectedPrefixName();
            string all = GetAllPrefixNames();

            foreach (var line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"))
                {
                    line.Text = line.Text
                        .Replace("{0}", selected)
                        .Replace("{1}", all);
                }
            }
        }

        public string GetAllPrefixNames()
        {
            var pool = prefixChoices;
            if (pool.Length == 0)
                return null;

            List<string> names = new();

            foreach (int p in pool)
            {
                if (p >= 0 && p < Lang.prefix.Length && Lang.prefix[p] != null)
                    names.Add(Lang.prefix[p].Value);
            }

            return string.Join(", ", names);
        }

        public string GetSelectedPrefixName()
        {
            var pool = prefixChoices;
            if (pool.Length == 0)
                return null;

            int p = pool[chosenPrefix];

            if (p >= 0 && p < Lang.prefix.Length && Lang.prefix[p] != null)
                return Lang.prefix[p].Value;

            return null;
        }
        #endregion

        public bool IsUsableVoucher(Player player, Item itemBeingReforged, bool isAccessory)
        {
            return Item.favorited && CanApply(player, itemBeingReforged, isAccessory);
        }

        public override bool CanRightClick() => Main.keyState.PressingShift();

        public override void RightClick(Player player)
        {
            if (prefixChoices.Length == 0)
                return;

            chosenPrefix++;
            if (chosenPrefix >= prefixChoices.Length)
                chosenPrefix = 0;

            Item.NetStateChanged();
        }

        public override bool ConsumeItem(Player player) => false;

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(chosenPrefix);
        }

        public override void NetReceive(BinaryReader reader)
        {
            chosenPrefix = reader.ReadInt32();
        }

        public virtual bool CanApply(Player player, Item itemBeingReforged, bool isAccessory)
        {
            return true;
        }

        public virtual int GetPrefixForItem(Player player, Item itemBeingReforged, bool isAccessory)
        {
            if (prefixChoices.Length == 0)
                return -1;

            if (isAccessory)
            {
                if (Main.rand.NextBool())
                {
                    VoucherReforgeSystem.RollWon = true;
                    return prefixChoices[chosenPrefix];
                }
                else
                {
                    VoucherReforgeSystem.RollWon = false;
                    return ModContent.PrefixType<Friendly>();
                }
            }

            VoucherReforgeSystem.RollWon = true;
            return prefixChoices[chosenPrefix];
        }
    }

    public class CombatVoucher : VoucherItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 7);  // Sold by Shady Salesman
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool CanApply(Player player, Item itemBeingReforged, bool isAccessory)
        {
            return !isAccessory;
        }

        public override int GetPrefixForItem(Player player, Item itemBeingReforged, bool isAccessory)
        {
            int prefix = -1;
            var category = itemBeingReforged.GetPrefixCategories();

            if (!category.Any())
                return -1;

            if ((itemBeingReforged).knockBack == 0f) // Weapons which do not apply knockback universally have demonic as their best reforge
                prefix = PrefixID.Demonic;

            else if (itemBeingReforged.CountsAsClass<MeleeDamageClass>() && itemBeingReforged.type != ModContent.ItemType<TheBurningSky>())
            {
                bool weirdMelee = !category.Contains(PrefixCategory.Melee); // Weapons which deal melee damage but cannot recieve melee prefixes because Item.noUseGraphic is true

                if (weirdMelee || itemBeingReforged.CountsAsClass<MeleeNoSpeedDamageClass>())
                    prefix = PrefixID.Godly;
                else
                    prefix = PrefixID.Legendary;
            }
            else if (itemBeingReforged.CountsAsClass<RangedDamageClass>() || itemBeingReforged.type == ModContent.ItemType<TheBurningSky>())
                prefix = PrefixID.Unreal;
            else if (itemBeingReforged.CountsAsClass<MagicDamageClass>())
                prefix = PrefixID.Mythical;
            else if (itemBeingReforged.CountsAsClass<RogueDamageClass>())
                prefix = ModContent.PrefixType<Flawless>();
            else if (itemBeingReforged.CountsAsClass<SummonDamageClass>())
                prefix = itemBeingReforged.IsWhip() ? PrefixID.Legendary : PrefixID.Ruthless;

            else // Fallback case
                prefix = PrefixID.Godly;

            if (Main.rand.NextBool())
            {
                VoucherReforgeSystem.RollWon = true;
                return prefix;
            }
            else
            {
                VoucherReforgeSystem.RollWon = false;
                return ModContent.PrefixType<Horrible>();
            }
        }
    }

    public class AggressiveVoucher : VoucherItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";

        protected override int[] prefixChoices => new int[]
        {
            PrefixID.Jagged,
            PrefixID.Angry,
            PrefixID.Spiked,
            PrefixID.Menacing,
            PrefixID.Precise,
            PrefixID.Lucky
        };

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 7);  // Sold by Shady Salesman
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool CanApply(Player player, Item itemBeingReforged, bool isAccessory)
        {
            return isAccessory;
        }
    }

    public class UnbreakableVoucher : VoucherItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";

        protected override int[] prefixChoices => new int[]
        {
            PrefixID.Hard,
            PrefixID.Armored,
            PrefixID.Guarding,
            PrefixID.Warding,
            ModContent.PrefixType<Invigorating>(),
            ModContent.PrefixType<Dauntless>()
        };

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 7);  // Sold by Shady Salesman
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool CanApply(Player player, Item itemBeingReforged, bool isAccessory)
        {
            return isAccessory;
        }
    }

    public class HurriedVoucher : VoucherItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";

        protected override int[] prefixChoices => new int[]
        {
            PrefixID.Brisk,
            PrefixID.Fleeting,
            PrefixID.Hasty2,
            PrefixID.Quick2
        };

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 7);  // Sold by Shady Salesman
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool CanApply(Player player, Item itemBeingReforged, bool isAccessory)
        {
            return isAccessory;
        }
    }

    public class OddVoucher : VoucherItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";

        protected override int[] prefixChoices => new int[]
        {
            PrefixID.Arcane,
            PrefixID.Wild,
            PrefixID.Rash,
            PrefixID.Intrepid,
            PrefixID.Violent,
            ModContent.PrefixType<Silent>()
        };

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 7);  // Sold by Shady Salesman
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool CanApply(Player player, Item itemBeingReforged, bool isAccessory)
        {
            return isAccessory;
        }
    }
}
