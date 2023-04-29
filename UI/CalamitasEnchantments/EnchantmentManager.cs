using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Other;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.UI.CalamitasEnchants
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class EnchantmentManager
    {
        internal const int ClearEnchantmentID = -18591774;
        internal const string ExhumedNamePath = "UI.Exhumed.DisplayName";

        public static List<Enchantment> EnchantmentList { get; internal set; } = new List<Enchantment>();
        public static Dictionary<int, int> ItemUpgradeRelationship { get; internal set; } = new Dictionary<int, int>();
        public static Enchantment ClearEnchantment { get; internal set; }
        public static IEnumerable<Enchantment> GetValidEnchantmentsForItem(Item item)
        {
            // Do nothing if the item cannot be enchanted.
            if (item is null || item.IsAir || !item.CanBeEnchantedBySomething())
                yield break;

            // Don't allow any enchantments if the item is an upgrade item.
            if (ItemUpgradeRelationship.ContainsValue(item.type))
                yield break;

            // Only give the option to clear if the item already has an enchantment.
            if (item.Calamity().AppliedEnchantment.HasValue)
            {
                yield return ClearEnchantment;
                yield break;
            }

            // Check based on all the requirements for all enchantments.
            foreach (Enchantment enchantment in EnchantmentList)
            {
                // Don't incorporate an enchantment in the list if the item already has it.
                if (item.Calamity().AppliedEnchantment.HasValue && item.Calamity().AppliedEnchantment.Value.Equals(enchantment))
                    continue;

                if (enchantment.ApplyRequirement(item))
                    yield return enchantment;
            }
        }

        public static Enchantment? FindByID(int id)
        {
            Enchantment? enchantment = EnchantmentList.FirstOrDefault(enchant => enchant.ID == id);
            if (enchantment.HasValue && !enchantment.Value.Equals(default(Enchantment)))
                return enchantment;
            return null;
        }

        public static void ConstructFromModcall(IEnumerable<object> parameters)
        {
            int secondaryArgumentCount = parameters.Count();
            if (secondaryArgumentCount < 5)
                throw new ArgumentNullException("ERROR: A minimum of 4 arguments must be supplied to this command; a name, a description, an id, and a requirement predicate.");
            if (secondaryArgumentCount > 7)
                throw new ArgumentNullException("ERROR: A maximum of 6 arguments can be supplied to this command.");

            string name = string.Empty;
            string description = string.Empty;
            int id = -1;
            string iconTexturePath = null;
            Predicate<Item> requirement = null;
            Action<Item> creationEffect = null;
            Action<Player> holdEffect = null;

            // First element - the name.
            if (parameters.ElementAt(0) is string nameElement)
                name = nameElement;
            else
                throw new ArgumentException("The first argument to this command must be a string.");

            // Second element - the description.
            if (parameters.ElementAt(1) is string descriptionElement)
                description = descriptionElement;
            else
                throw new ArgumentException("The second argument to this command must be a string.");

            // Third element - the ID.
            if (parameters.ElementAt(2) is int idElement)
                id = idElement;
            else
                throw new ArgumentException("The third argument to this command must be an int.");

            // Fourth element - the requirement predicate. This determines if an item can be enchanted by said enchant or not.
            if (parameters.ElementAt(3) is Predicate<Item> requirementElement)
                requirement = requirementElement;
            else
                throw new ArgumentException("The fourth argument to this command must be an Item Predicate.");

            // Fifth element - the texture path. This determines what icon is drawn on the UI when selected. It may be null to specify that none should be drawn.
            if (parameters.ElementAt(4) is string iconTexturePathElement)
                iconTexturePath = iconTexturePathElement;
            else
                throw new ArgumentException("The fifth argument to this command must be a string.");

            // Optional elements - creation and hold effects.
            switch (secondaryArgumentCount)
            {
                case 6:
                    object sixthElement = parameters.ElementAt(5);
                    if (sixthElement is Action<Item> creationElement)
                        creationEffect = creationElement;
                    else if (sixthElement is Action<Player> holdElement)
                        holdEffect = holdElement;
                    else
                        throw new ArgumentException("The sixth argument to this command must be an Item or Player Action.");
                    break;
                case 7:
                    sixthElement = parameters.ElementAt(5);
                    object seventhElement = parameters.ElementAt(6);
                    if (sixthElement is Action<Item> creationElement2)
                    {
                        creationEffect = creationElement2;
                        holdEffect = seventhElement as Action<Player>;
                    }
                    else if (sixthElement is Action<Player> holdElement2)
                    {
                        creationEffect = seventhElement as Action<Item>;
                        holdEffect = holdElement2;
                    }
                    else
                        throw new ArgumentException("The sixth argument to this command must be an Item or Player Action and the sixth must be the other action type.");
                    break;
            }

            // Ensure the enchantment's ID is not already claimed.
            if (EnchantmentList.Any(enchant => enchant.ID == id) || id == ClearEnchantmentID)
                throw new ArgumentException("An enchantment with this ID already exists. Another one must be specified.");

            EnchantmentList.Add(new Enchantment(name, description, id, iconTexturePathElement, creationEffect, holdEffect, requirement));
        }

        internal static void LoadAllEnchantments()
        {
            EnchantmentList = new List<Enchantment>
            {
                new Enchantment(CalamityUtils.GetTextValue(ExhumedNamePath), CalamityUtils.GetTextValue("UI.Exhumed.Description"),
                    1,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Exhumed",
                    null,
                    null,
                    item => ItemUpgradeRelationship.ContainsKey(item.type)),

                new Enchantment(CalamityUtils.GetTextValue("UI.Indignant.DisplayName"), CalamityUtils.GetTextValue("UI.Indignant.Description"),
                    100,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Indignant",
                    null,
                    player => player.Calamity().cursedSummonsEnchant = true,
                    item => item.IsEnchantable() && item.damage > 0 && item.CountsAsClass<SummonDamageClass>() && !item.IsWhip()),

                new Enchantment(CalamityUtils.GetTextValue("UI.Aflame.DisplayName"), CalamityUtils.GetTextValue("UI.Aflame.Description"),
                    200,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Aflame",
                    null,
                    player => player.Calamity().flamingItemEnchant = true,
                    item => item.IsEnchantable() && item.damage > 0 && !item.CountsAsClass<SummonDamageClass>() && !item.IsWhip()),

                new Enchantment(CalamityUtils.GetTextValue("UI.Oblatory.DisplayName"), CalamityUtils.GetTextValue("UI.Oblatory.Description"),
                    300,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Oblatory",
                    item =>
                    {
                        item.damage = (int)(item.damage * 1.25);
                        item.mana = (int)Math.Ceiling(item.mana * 0.7);
                    },
                    player => player.Calamity().lifeManaEnchant = true,
                    item => item.IsEnchantable() && item.damage > 0 && item.CountsAsClass<MagicDamageClass>() && item.mana > 0 && item.type != ModContent.ItemType<Eternity>()),

                new Enchantment(CalamityUtils.GetTextValue("UI.Resentful.DisplayName"), CalamityUtils.GetTextValue("UI.Resentful.Description"),
                    400,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Resentful",
                    null,
                    player => player.Calamity().farProximityRewardEnchant = true,
                    item => item.IsEnchantable() && item.damage > 0 && item.shoot > ProjectileID.None && !item.IsTrueMelee() && item.type != ModContent.ItemType<FinalDawn>()),

                new Enchantment(CalamityUtils.GetTextValue("UI.Bloodthirsty.DisplayName"), CalamityUtils.GetTextValue("UI.Bloodthirsty.Description"),
                    500,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Bloodthirsty",
                    null,
                    player => player.Calamity().closeProximityRewardEnchant = true,
                    item => item.IsEnchantable() && item.damage > 0 && item.shoot > ProjectileID.None && !item.IsTrueMelee() && item.type != ModContent.ItemType<FinalDawn>()),

                new Enchantment(CalamityUtils.GetTextValue("UI.Ephemeral.DisplayName"), CalamityUtils.GetTextValue("UI.Ephemeral.Description"),
                    600,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Ephemeral",
                    null,
                    player => player.Calamity().dischargingItemEnchant = true,
                    item => item.IsEnchantable() && item.damage > 0 && !item.CountsAsClass<SummonDamageClass>() && !item.CountsAsClass<RogueDamageClass>() &&
                    !item.channel && item.type != ModContent.ItemType<HeavenlyGale>()),

                new Enchantment(CalamityUtils.GetTextValue("UI.Hellbound.DisplayName"), CalamityUtils.GetTextValue("UI.Hellbound.Description"),
                    700,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Hellbound",
                    null,
                    player => player.Calamity().explosiveMinionsEnchant = true,
                    item => item.IsEnchantable() && item.damage > 0 && item.CountsAsClass<SummonDamageClass>() && !item.IsWhip()),

                new Enchantment(CalamityUtils.GetTextValue("UI.Tainted.DisplayName"), CalamityUtils.GetTextValue("UI.Tainted.Description"),
                    800,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Tainted",
                    item => item.useTime = item.useAnimation = 25,
                    (Player player) =>
                    {
                        if (Main.gameMenu)
                            return;

                        player.Calamity().bladeArmEnchant = true;
                        bool armsArePresent = false;
                        int armType = ModContent.ProjectileType<TaintedBladeSlasher>();
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].type != armType || Main.projectile[i].owner != player.whoAmI || !Main.projectile[i].active)
                                continue;

                            armsArePresent = true;
                            break;
                        }

                        if (Main.myPlayer == player.whoAmI && !armsArePresent)
                        {
                            // Yes, this is a LOT of damage but given the limited range of this thing it needs to be extremely powerful when it does actually hit.
                            var source = player.GetSource_ItemUse(player.ActiveItem());
                            float taintedRatio = 5f;
                            int damage = (int)player.GetTotalDamage<MeleeDamageClass>().ApplyTo(player.ActiveItem().damage * taintedRatio);
                            int blade = Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<TaintedBladeSlasher>(), damage, 0f, player.whoAmI, 0f, player.ActiveItem().type);
                            if (Main.projectile.IndexInRange(blade))
                                Main.projectile[blade].localAI[0] = 0f;

                            blade = Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<TaintedBladeSlasher>(), damage, 0f, player.whoAmI, 1f, player.ActiveItem().type);
                            if (Main.projectile.IndexInRange(blade))
                                Main.projectile[blade].localAI[0] = -80f;
                        }
                    },
                    item => item.IsEnchantable() && item.damage > 0 && item.CountsAsClass<MeleeDamageClass>() && !item.noUseGraphic && item.shoot > ProjectileID.None),

                new Enchantment(CalamityUtils.GetTextValue("UI.Traitorous.DisplayName"), CalamityUtils.GetTextValue("UI.Traitorous.Description"),
                    900,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Traitorous",
                    null,
                    player => player.Calamity().manaMonsterEnchant = true,
                    item => item.IsEnchantable() && item.damage > 0 && item.CountsAsClass<MagicDamageClass>() && item.mana > 0),

                new Enchantment(CalamityUtils.GetTextValue("UI.Withering.DisplayName"), CalamityUtils.GetTextValue("UI.Withering.Description"),
                    1000,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Withered",
                    null,
                    player => player.Calamity().witheringWeaponEnchant = true,
                    item => item.IsEnchantable() && item.damage > 0 && !item.CountsAsClass<SummonDamageClass>()),

                new Enchantment(CalamityUtils.GetTextValue("UI.Persecuted.DisplayName"), CalamityUtils.GetTextValue("UI.Persecuted.Description"),
                    1100,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Persecuted",
                    null,
                    player => player.Calamity().persecutedEnchant = true,
                    item => item.IsEnchantable() && item.damage > 0 && item.shoot > ProjectileID.None),

                new Enchantment(CalamityUtils.GetTextValue("UI.Lecherous.DisplayName"), CalamityUtils.GetTextValue("UI.Lecherous.Description"),
                    1200,
                    "CalamityMod/UI/CalamitasEnchantments/CurseIcon_Lecherous",
                    null,
                    player =>
                    {
                        if (Main.gameMenu)
                            return;

                        player.Calamity().lecherousOrbEnchant = true;

                        bool orbIsPresent = false;
                        int orbType = ModContent.NPCType<LecherousOrb>();
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].type != orbType || Main.npc[i].target != player.whoAmI || !Main.npc[i].active)
                                continue;

                            orbIsPresent = true;
                            break;
                        }

                        if (Main.myPlayer == player.whoAmI && !orbIsPresent && !player.Calamity().awaitingLecherousOrbSpawn)
                        {
                            player.Calamity().awaitingLecherousOrbSpawn = true;
                            CalamityNetcode.NewNPC_ClientSide(player.Center, orbType, player);
                        }
                    },
                    item => item.IsEnchantable() && item.damage > 0 && item.shoot > ProjectileID.None && !item.CountsAsClass<SummonDamageClass>() && !item.IsTrueMelee()),
            };

            // Special disenchantment thing. This is separated from the list on purpose.
            ClearEnchantment = new Enchantment(CalamityUtils.GetTextValue("UI.Disenchant"),
                string.Empty,
                ClearEnchantmentID,
                null,
                item =>
                {
                    item.Calamity().AppliedEnchantment = null;
                    item.Calamity().DischargeEnchantExhaustion = 0;
                },
                item => item.IsEnchantable() && item.shoot >= ProjectileID.None);

            ItemUpgradeRelationship = new Dictionary<int, int>()
            {
                [ModContent.ItemType<TheCommunity>()] = ModContent.ItemType<ShatteredCommunity>(),
                [ModContent.ItemType<EntropysVigil>()] = ModContent.ItemType<CindersOfLament>(),
                [ModContent.ItemType<StaffoftheMechworm>()] = ModContent.ItemType<Metastasis>(),
                [ModContent.ItemType<GhastlyVisage>()] = ModContent.ItemType<GruesomeEminence>(),
                [ModContent.ItemType<BurningSea>()] = ModContent.ItemType<Rancor>()
            };
        }

        internal static void UnloadAllEnchantments()
        {
            EnchantmentList = null;
            ItemUpgradeRelationship = null;
        }
    }
}
