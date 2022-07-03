using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Accessories.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class AbandonnedWulfrumHelmet : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Vanity/AbandonnedWulfrumHelmet_HeadSet", EquipType.Head, name: "WulfrumOldSetHead");
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Vanity/AbandonnedWulfrumHelmet_Body", EquipType.Body, this);
                EquipLoader.AddEquipTexture(Mod, "CalamityMod/Items/Accessories/Vanity/AbandonnedWulfrumHelmet_Legs", EquipType.Legs, this);
            }
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Abandonned Wulfrum Helmet");
            Tooltip.SetDefault("A worn and rusty helmet ressembling older models of wulfrum armor\n" +
                //Could include lore about how X kind of people used to wear it. Like "Streets used to be filled with people of The Resistance wearing this cheap yet effective armor.
                "Transforms the holder into a wulfrum robot\n" +
                "Can also be worn in the helmet slot as a regular helm");

            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, "WulfrumOldSetHead", EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;

            int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;

            int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.vanity = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<WulfrumTransformationPlayer>().vanityEquiped = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
                player.GetModPlayer<WulfrumTransformationPlayer>().vanityEquiped = true;
        }
    }

    public class WulfrumTransformationPlayer : ModPlayer
    {
        public bool vanityEquiped = false;
        public int hurtSoundTimer;

        public override void ResetEffects()
        {
            vanityEquiped = false;
        }

        public override void FrameEffects()
        {
            if (vanityEquiped)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "AbandonnedWulfrumHelmet", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "AbandonnedWulfrumHelmet", EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, "WulfrumOldSetHead", EquipType.Head);
                Player.face = -1;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (hurtSoundTimer > 0)
                hurtSoundTimer--;
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (vanityEquiped && hurtSoundTimer == 0)
            {
                playSound = false;
                SoundEngine.PlaySound(SoundID.NPCHit4, Player.position);
                hurtSoundTimer = 10;
            }

            return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
        }
    }
}
