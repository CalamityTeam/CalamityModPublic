using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod
{
    public static class CalamityUtils
    {
        #region Object Extensions
        public static CalamityPlayer Calamity(this Player player) => player.GetModPlayer<CalamityPlayer>();
        public static CalamityGlobalNPC Calamity(this NPC npc) => npc.GetGlobalNPC<CalamityGlobalNPC>();
        public static CalamityGlobalItem Calamity(this Item item) => item.GetGlobalItem<CalamityGlobalItem>();
        public static CalamityGlobalProjectile Calamity(this Projectile proj) => proj.GetGlobalProjectile<CalamityGlobalProjectile>();
        #endregion

        #region Player Utilities
        public static bool InCalamity(this Player player) => player.Calamity().ZoneCalamity;
        public static bool InAstral(this Player player) => player.Calamity().ZoneAstral;
        public static bool InSunkenSea(this Player player) => player.Calamity().ZoneSunkenSea;
        public static bool InSulphur(this Player player) => player.Calamity().ZoneSulphur;
        public static bool InAbyss(this Player player, int layer = 0)
        {
            switch (layer)
            {
                case 1:
                    return player.Calamity().ZoneAbyssLayer1;

                case 2:
                    return player.Calamity().ZoneAbyssLayer2;

                case 3:
                    return player.Calamity().ZoneAbyssLayer3;

                case 4:
                    return player.Calamity().ZoneAbyssLayer4;

                default:
                    return player.Calamity().ZoneAbyss;
            }
        }

        public static bool InventoryHas(this Player player, params int[] items)
        {
            return player.inventory.Any(item => items.Contains(item.type));
        }
        #endregion

        #region NPC Utilities
        /// <summary>
        /// Allows you to set the lifeMax value of a NPC to different values based on the mode. Called instead of npc.lifeMax = X.
        /// </summary>
        /// <param name="npc">The NPC whose lifeMax value you are trying to set.</param>
        /// <param name="normal">The value lifeMax will be set to in normal mode, this value gets doubled automatically in Expert mode.</param>
        /// <param name="revengeance">The value lifeMax will be set to in Revegeneance mode.</param>
        /// <param name="death">The value lifeMax will be set to in Death mode.</param>
        /// <param name="bossRush">The value lifeMax will be set to during the Boss Rush.</param>
        /// <param name="bossRushDeath">The value lifeMax will be set to during the Boss Rush, if Death mode is active.</param>
        public static void LifeMaxNERD(this NPC npc, int normal, int? revengeance = null, int? death = null, int? bossRush = null, int? bossRushDeath = null)
        {
            npc.lifeMax = normal;

            if (bossRush.HasValue && CalamityWorld.bossRushActive)
            {
                npc.lifeMax = bossRushDeath.HasValue && CalamityWorld.death ? bossRushDeath.Value : bossRush.Value;
            }
            else if (death.HasValue && CalamityWorld.death)
            {
                npc.lifeMax = death.Value;
            }
            else if (revengeance.HasValue && CalamityWorld.revenge)
            {
                npc.lifeMax = revengeance.Value;
            }
            if (npc.boss)
            {
                double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
                npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
            }
        }
        /// <summary>
        /// Detects nearby hostile NPCs from a given point
        /// </summary>
        /// <param name="origin">The position where we wish to check for nearby NPCs</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        public static NPC ClosestNPCAt(this Vector2 origin, float maxDistanceToCheck)
        {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            for (int index = 0; index < Main.npc.Length; index++)
            {
                //doesn't matter what the attacker is in CanBeChasedBy? wtf.
                if (Main.npc[index].CanBeChasedBy(null, false) && Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1))
                {
                    if (Vector2.Distance(origin, Main.npc[index].Center) < distance)
                    {
                        distance = Vector2.Distance(origin, Main.npc[index].Center);
                        closestTarget = Main.npc[index];
                    }
                }
            }
            return closestTarget;
        }
        /// <summary>
        /// Detects nearby hostile NPCs from a given point with minion support
        /// </summary>
        /// <param name="origin">The position where we wish to check for nearby NPCs</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        /// <param name="owner">Owner of the minion</param>
        public static NPC MinionHoming(this Vector2 origin, float maxDistanceToCheck, Player owner)
        {
            if (owner.HasMinionAttackTargetNPC)
            {
                return Main.npc[owner.MinionAttackTargetNPC];
            }
            return ClosestNPCAt(origin, maxDistanceToCheck);
        }

        /// <summary>
        /// Crude anti-butcher logic based on % max health.
        /// </summary>
        /// <param name="npc">The NPC attacked.</param>
        /// <param name="damage">How much damage the attack would deal.</param>
        /// <returns>Whether or not the anti-butcher was triggered.</returns>
        public static bool AntiButcher(NPC npc, ref double damage, float healthPercent)
        {
            if (damage <= npc.lifeMax * healthPercent)
                return false;
            damage = 0D;
            return true;
        }
        #endregion

        #region Item Utilities
        public static Rectangle FixSwingHitbox(float hitboxWidth, float hitboxHeight)
        {
            Player player = Main.player[Main.myPlayer];
            Item item = player.inventory[player.selectedItem];
            float hitbox_X = 0, hitbox_Y = 0;
            float mountOffsetY = player.mount.PlayerOffsetHitbox;

            // Third hitbox shifting values
            if (player.itemAnimation < player.itemAnimationMax * 0.333)
            {
                float shiftX = 10f;
                if (hitboxWidth >= 92)
                    shiftX = 38f;
                else if (hitboxWidth >= 64)
                    shiftX = 28f;
                else if (hitboxWidth >= 52)
                    shiftX = 24f;
                else if (hitboxWidth > 32)
                    shiftX = 14f;
                hitbox_X = player.position.X + player.width * 0.5f + (hitboxWidth * 0.5f - shiftX) * player.direction;
                hitbox_Y = player.position.Y + 24f + mountOffsetY;
            }

            // Second hitbox shifting values
            else if (player.itemAnimation < player.itemAnimationMax * 0.666)
            {
                float shift = 10f;
                if (hitboxWidth >= 92)
                    shift = 38f;
                else if (hitboxWidth >= 64)
                    shift = 28f;
                else if (hitboxWidth >= 52)
                    shift = 24f;
                else if (hitboxWidth > 32)
                    shift = 18f;
                hitbox_X = player.position.X + (player.width * 0.5f + (hitboxWidth * 0.5f - shift) * player.direction);

                shift = 10f;
                if (hitboxHeight > 64)
                    shift = 14f;
                else if (hitboxHeight > 52)
                    shift = 12f;
                else if (hitboxHeight > 32)
                    shift = 8f;

                hitbox_Y = player.position.Y + shift + mountOffsetY;
            }

            // First hitbox shifting values
            else
            {
                float shift = 6f;
                if (hitboxWidth >= 92)
                    shift = 38f;
                else if (hitboxWidth >= 64)
                    shift = 28f;
                else if (hitboxWidth >= 52)
                    shift = 24f;
                else if (hitboxWidth >= 48)
                    shift = 18f;
                else if (hitboxWidth > 32)
                    shift = 14f;
                hitbox_X = player.position.X + player.width * 0.5f - (hitboxWidth * 0.5f - shift) * player.direction;

                shift = 10f;
                if (hitboxHeight > 64)
                    shift = 14f;
                else if (hitboxHeight > 52)
                    shift = 12f;
                else if (hitboxHeight > 32)
                    shift = 10f;
                hitbox_Y = player.position.Y + shift + mountOffsetY;
            }

            // Inversion due to grav potion
            if (player.gravDir == -1f)
            {
                hitbox_Y = player.position.Y + player.height + (player.position.Y - hitbox_Y);
            }

            // Hitbox size adjustments
            Rectangle hitbox = new Rectangle((int)hitbox_X, (int)hitbox_Y, 32, 32);
            if (item.damage >= 0 && item.type > 0 && !item.noMelee && player.itemAnimation > 0)
            {
                if (!Main.dedServ)
                {
                    hitbox = new Rectangle((int)hitbox_X, (int)hitbox_Y, (int)hitboxWidth, (int)hitboxHeight);
                }
                hitbox.Width = (int)(hitbox.Width * item.scale);
                hitbox.Height = (int)(hitbox.Height * item.scale);
                if (player.direction == -1)
                {
                    hitbox.X -= hitbox.Width;
                }
                if (player.gravDir == 1f)
                {
                    hitbox.Y -= hitbox.Height;
                }

                // Broadsword use style
                if (item.useStyle == 1)
                {
                    // Third hitbox size adjustments
                    if (player.itemAnimation < player.itemAnimationMax * 0.333)
                    {
                        if (player.direction == -1)
                        {
                            hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);
                        }
                        hitbox.Width = (int)(hitbox.Width * 1.4);
                        hitbox.Y += (int)(hitbox.Height * 0.5 * player.gravDir);
                        hitbox.Height = (int)(hitbox.Height * 1.1);
                    }

                    // First hitbox size adjustments
                    else if (player.itemAnimation >= player.itemAnimationMax * 0.666)
                    {
                        if (player.direction == 1)
                        {
                            hitbox.X -= (int)(hitbox.Width * 1.2);
                        }
                        hitbox.Width *= 2;
                        hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
                        hitbox.Height = (int)(hitbox.Height * 1.4);
                    }
                }
            }
            return hitbox;
        }
        #endregion

        #region Projectile Utilities
        public static int CountProjectiles(int Type) => Main.projectile.Count(proj => proj.type == Type && proj.active);
        #endregion

        #region Tile Utilities
        public static void LightHitWire(int type, int i, int j, int tileX, int tileY)
        {
            int x = i - Main.tile[i, j].frameX / 18 % tileX;
            int y = j - Main.tile[i, j].frameY / 18 % tileY;
            for (int l = x; l < x + tileX; l++)
            {
                for (int m = y; m < y + tileY; m++)
                {
                    if (Main.tile[l, m] == null)
                    {
                        Main.tile[l, m] = new Tile();
                    }
                    if (Main.tile[l, m].active() && Main.tile[l, m].type == type)
                    {
                        if (Main.tile[l, m].frameX < (18 * tileX))
                        {
                            Main.tile[l, m].frameX += (short)(18 * tileX);
                        }
                        else
                        {
                            Main.tile[l, m].frameX -= (short)(18 * tileX);
                        }
                    }
                }
            }
            if (Wiring.running)
            {
                for (int k = 0; k < tileX; k++)
                {
                    for (int l = 0; l < tileY; l++)
                    {
                        Wiring.SkipWire(x + k, y + l);
                    }
                }
            }
        }
        public static bool BedRightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int spawnX = i - tile.frameX / 18;
            int spawnY = j + 2;
            spawnX += tile.frameX >= 72 ? 5 : 2;
            if (tile.frameY % 38 != 0)
            {
                spawnY--;
            }
            player.FindSpawn();
            if (player.SpawnX == spawnX && player.SpawnY == spawnY)
            {
                player.RemoveSpawn();
                Main.NewText("Spawn point removed!", 255, 240, 20, false);
            }
            else if (Player.CheckSpawn(spawnX, spawnY))
            {
                player.ChangeSpawn(spawnX, spawnY);
                Main.NewText("Spawn point set!", 255, 240, 20, false);
            }
            return true;
        }
        public static bool ChestRightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            Main.mouseRightRelease = false;
            int left = i;
            int top = j;
            if (tile.frameX % 36 != 0)
            {
                left--;
            }
            if (tile.frameY != 0)
            {
                top--;
            }
            if (player.sign >= 0)
            {
                Main.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }
            if (Main.editChest)
            {
                Main.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }
            if (player.editedChestName)
            {
                NetMessage.SendData(33, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                player.editedChestName = false;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (left == player.chestX && top == player.chestY && player.chest >= 0)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    Main.PlaySound(SoundID.MenuClose);
                }
                else
                {
                    NetMessage.SendData(31, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
                    Main.stackSplit = 600;
                }
            }
            else
            {
                int chest = Chest.FindChest(left, top);
                if (chest >= 0)
                {
                    Main.stackSplit = 600;
                    if (chest == player.chest)
                    {
                        player.chest = -1;
                        Main.PlaySound(SoundID.MenuClose);
                    }
                    else
                    {
                        player.chest = chest;
                        Main.playerInventory = true;
                        Main.recBigList = false;
                        player.chestX = left;
                        player.chestY = top;
                        Main.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                    }
                    Recipe.FindRecipes();
                }
            }
            return true;
        }
        public static void ChestMouseOver(string itemType, string name, int i, int j)
        {
            Mod calamity = ModLoader.GetMod("CalamityMod");
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;
            if (tile.frameX % 36 != 0)
            {
                left--;
            }
            if (tile.frameY != 0)
            {
                top--;
            }
            int chest = Chest.FindChest(left, top);
            player.showItemIcon2 = -1;
            if (chest < 0)
            {
                player.showItemIconText = Language.GetTextValue("LegacyChestType.0");
            }
            else
            {
                player.showItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : name;
                if (player.showItemIconText == name)
                {
                    player.showItemIcon2 = calamity.ItemType(itemType);
                    player.showItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.showItemIcon = true;
        }
        public static void ChestMouseFar(string itemType, string name, int i, int j)
        {
            ChestMouseOver(itemType, name, i, j);
            Player player = Main.LocalPlayer;
            if (player.showItemIconText == "")
            {
                player.showItemIcon = false;
                player.showItemIcon2 = 0;
            }
        }
        public static bool ClockRightClick()
        {
            string text = "AM";
            //Get current weird time
            double time = Main.time;
            if (!Main.dayTime)
            {
                //if it's night add this number
                time += 54000.0;
            }
            //Divide by seconds in a day * 24
            time = time / 86400.0 * 24.0;
            //Dunno why we're taking 19.5. Something about hour formatting
            time = time - 7.5 - 12.0;
            //Format in readable time
            if (time < 0.0)
            {
                time += 24.0;
            }
            if (time >= 12.0)
            {
                text = "PM";
            }
            int intTime = (int)time;
            //Get the decimal points of time.
            double deltaTime = time - intTime;
            //multiply them by 60. Minutes, probably
            deltaTime = (int)(deltaTime * 60.0);
            //This could easily be replaced by deltaTime.ToString()
            string text2 = string.Concat(deltaTime);
            if (deltaTime < 10.0)
            {
                //if deltaTime is eg "1" (which would cause time to display as HH:M instead of HH:MM)
                text2 = "0" + text2;
            }
            if (intTime > 12)
            {
                //This is for AM/PM time rather than 24hour time
                intTime -= 12;
            }
            if (intTime == 0)
            {
                //0AM = 12AM
                intTime = 12;
            }
            //Whack it all together to get a HH:MM format
            var newText = string.Concat("Time: ", intTime, ":", text2, " ", text);
            Main.NewText(newText, 255, 240, 20);
            return true;
        }
        public static bool DresserRightClick()
        {
            Player player = Main.LocalPlayer;
            if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameY == 0)
            {
                Main.CancelClothesWindow(true);

                int left = (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameX / 18);
                left %= 3;
                left = Player.tileTargetX - left;
                int top = Player.tileTargetY - (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameY / 18);
                if (player.sign > -1)
                {
                    Main.PlaySound(SoundID.MenuClose);
                    player.sign = -1;
                    Main.editSign = false;
                    Main.npcChatText = string.Empty;
                }
                if (Main.editChest)
                {
                    Main.PlaySound(SoundID.MenuTick);
                    Main.editChest = false;
                    Main.npcChatText = string.Empty;
                }
                if (player.editedChestName)
                {
                    NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                    player.editedChestName = false;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    if (left == player.chestX && top == player.chestY && player.chest != -1)
                    {
                        player.chest = -1;
                        Recipe.FindRecipes();
                        Main.PlaySound(SoundID.MenuClose);
                    }
                    else
                    {
                        NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
                        Main.stackSplit = 600;
                    }
                    return true;
                }
                else
                {
                    player.flyingPigChest = -1;
                    int num213 = Chest.FindChest(left, top);
                    if (num213 != -1)
                    {
                        Main.stackSplit = 600;
                        if (num213 == player.chest)
                        {
                            player.chest = -1;
                            Recipe.FindRecipes();
                            Main.PlaySound(SoundID.MenuClose);
                        }
                        else if (num213 != player.chest && player.chest == -1)
                        {
                            player.chest = num213;
                            Main.playerInventory = true;
                            Main.recBigList = false;
                            Main.PlaySound(SoundID.MenuOpen);
                            player.chestX = left;
                            player.chestY = top;
                        }
                        else
                        {
                            player.chest = num213;
                            Main.playerInventory = true;
                            Main.recBigList = false;
                            Main.PlaySound(SoundID.MenuTick);
                            player.chestX = left;
                            player.chestY = top;
                        }
                        Recipe.FindRecipes();
                        return true;
                    }
                }
            }
            else
            {
                Main.playerInventory = false;
                player.chest = -1;
                Recipe.FindRecipes();
                Main.dresserX = Player.tileTargetX;
                Main.dresserY = Player.tileTargetY;
                Main.OpenClothesWindow();
                return true;
            }

            return false;
        }
        public static void DresserMouseFar(string itemType, string chest, int i, int j)
        {
            Mod calamity = ModLoader.GetMod("CalamityMod");
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            int left = Player.tileTargetX;
            int top = Player.tileTargetY;
            left -= (int)(tile.frameX % 54 / 18);
            if (tile.frameY % 36 != 0)
            {
                top--;
            }
            int chestIndex = Chest.FindChest(left, top);
            player.showItemIcon2 = -1;
            if (chestIndex < 0)
            {
                player.showItemIconText = Language.GetTextValue("LegacyDresserType.0");
            }
            else
            {
                if (Main.chest[chestIndex].name != "")
                {
                    player.showItemIconText = Main.chest[chestIndex].name;
                }
                else
                {
                    player.showItemIconText = chest;
                }
                if (player.showItemIconText == chest)
                {
                    player.showItemIcon2 = calamity.ItemType(itemType);
                    player.showItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.showItemIcon = true;
            if (player.showItemIconText == "")
            {
                player.showItemIcon = false;
                player.showItemIcon2 = 0;
            }
        }
        public static void DresserMouseOver(string itemType, string chest, int i, int j)
        {
            Mod calamity = ModLoader.GetMod("CalamityMod");
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            int left = Player.tileTargetX;
            int top = Player.tileTargetY;
            left -= (int)(tile.frameX % 54 / 18);
            if (tile.frameY % 36 != 0)
            {
                top--;
            }
            int num138 = Chest.FindChest(left, top);
            player.showItemIcon2 = -1;
            if (num138 < 0)
            {
                player.showItemIconText = Language.GetTextValue("LegacyDresserType.0");
            }
            else
            {
                if (Main.chest[num138].name != "")
                {
                    player.showItemIconText = Main.chest[num138].name;
                }
                else
                {
                    player.showItemIconText = chest;
                }
                if (player.showItemIconText == chest)
                {
                    player.showItemIcon2 = calamity.ItemType(itemType);
                    player.showItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.showItemIcon = true;
            if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameY > 0)
            {
                player.showItemIcon2 = ItemID.FamiliarShirt;
            }
        }
        public static bool LockedChestRightClick(bool isLocked, int left, int top, int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];

            // If the player right clicked the chest while editing a sign, finish that up
            if (player.sign >= 0)
            {
                Main.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }

            // If the player right clicked the chest while editing a chest, finish that up
            if (Main.editChest)
            {
                Main.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }

            // If the player right clicked the chest after changing another chest's name, finish that up
            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
                player.editedChestName = false;
            }
            if (Main.netMode == 1 && !isLocked)
            {
                // Right clicking the chest you currently have open closes it. This counts as interaction.
                if (left == player.chestX && top == player.chestY && player.chest >= 0)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    Main.PlaySound(SoundID.MenuClose);
                }

                // Right clicking this chest opens it if it's not already open. This counts as interaction.
                else
                {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
                    Main.stackSplit = 600;
                }
                return true;
            }


            else
            {
                if (isLocked)
                {
                    // If you right click the locked chest and you can unlock it, it unlocks itself but does not open. This counts as interaction.
                    if (Chest.Unlock(left, top))
                    {
                        if (Main.netMode == 1)
                        {
                            NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, (float)left, (float)top);
                        }
                        return true;
                    }
                }
                else
                {
                    int chest = Chest.FindChest(left, top);
                    if (chest >= 0)
                    {
                        Main.stackSplit = 600;

                        // If you right click the same chest you already have open, it closes. This counts as interaction.
                        if (chest == player.chest)
                        {
                            player.chest = -1;
                            Main.PlaySound(SoundID.MenuClose);
                        }

                        // If you right click this chest when you have a different chest selected, that one closes and this one opens. This counts as interaction.
                        else
                        {
                            player.chest = chest;
                            Main.playerInventory = true;
                            Main.recBigList = false;
                            player.chestX = left;
                            player.chestY = top;
                            Main.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                        }

                        Recipe.FindRecipes();
                        return true;
                    }
                }
            }

            // This only occurs when the chest is locked and cannot be unlocked. You did not interact with the chest.
            return false;
        }
        public static void LockedChestMouseOver(string keyType, string chestType, string chestName, int i, int j)
        {
            Mod calamity = ModLoader.GetMod("CalamityMod");
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;
            if (tile.frameX % 36 != 0)
            {
                left--;
            }
            if (tile.frameY != 0)
            {
                top--;
            }
            int chest = Chest.FindChest(left, top);
            player.showItemIcon2 = -1;
            if (chest < 0)
            {
                player.showItemIconText = Language.GetTextValue("LegacyChestType.0");
            }
            else
            {
                player.showItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : chestName;
                if (player.showItemIconText == chestName)
                {
                    player.showItemIcon2 = calamity.ItemType(chestType);
                    if (Main.tile[left, top].frameX / 36 == 1)
                        player.showItemIcon2 = calamity.ItemType(keyType);
                    player.showItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.showItemIcon = true;
        }
        public static void LockedChestMouseOverFar(string keyType, string chestType, string chestName, int i, int j)
        {
            LockedChestMouseOver(keyType, chestType, chestName, i, j);
            Player player = Main.LocalPlayer;
            if (player.showItemIconText == "")
            {
                player.showItemIcon = false;
                player.showItemIcon2 = 0;
            }
        }

        #region Furniture SetDefaults
        public static void SetUpBathtub(int type, bool lavaImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 4, 0);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(type);
        }
        public static void SetUpBed(int type, bool lavaImmune = false)
        {
            Main.tileFrameImportant[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileID.Sets.HasOutlines[type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 4, 0);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(type);
        }
        public static void SetUpBookcase(int type, bool lavaImmune = false)
        {
            Main.tileSolidTop[type] = true;
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileTable[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpCandelabra(int type, bool lavaImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileLavaDeath[type] = true;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpCandle(int type, bool lavaImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpChair(int type, bool lavaImmune = false)
        {
            Main.tileFrameImportant[type] = true;
            Main.tileNoAttach[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(type);
        }
        public static void SetUpChandelier(int type, bool lavaImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileNoAttach[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpChest(int type)
        {
            Main.tileSpelunker[type] = true;
            Main.tileContainer[type] = true;
            Main.tileShine2[type] = true;
            Main.tileShine[type] = 1200;
            Main.tileFrameImportant[type] = true;
            Main.tileNoAttach[type] = true;
            Main.tileValue[type] = 500;
            TileID.Sets.HasOutlines[type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(type);
        }
        public static void SetUpClock(int type, bool lavaImmune = false)
        {
            Main.tileFrameImportant[type] = true;
            Main.tileNoAttach[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            TileID.Sets.HasOutlines[type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16,
                16,
                16,
                16
            };
            TileObjectData.newTile.Origin = new Point16(0, 4);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpDoorClosed(int type, bool lavaImmune = false)
        {
            Main.tileFrameImportant[type] = true;
            Main.tileBlockLight[type] = true;
            Main.tileSolid[type] = true;
            Main.tileNoAttach[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileID.Sets.NotReallySolid[type] = true;
            TileID.Sets.DrawsWalls[type] = true;
            TileID.Sets.HasOutlines[type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(type);
        }
        public static void SetUpDoorOpen(int type, bool lavaImmune = false)
        {
            Main.tileFrameImportant[type] = true;
            Main.tileSolid[type] = false;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            Main.tileNoSunLight[type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 0);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 1);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 2);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(type);
            TileID.Sets.HousingWalls[type] = true;
            TileID.Sets.HasOutlines[type] = true;
        }
        public static void SetUpDresser(int type)
        {
            Main.tileSolidTop[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileNoAttach[type] = true;
            Main.tileTable[type] = true;
            Main.tileContainer[type] = true;
            Main.tileWaterDeath[type] = false;
            Main.tileLavaDeath[type] = false;
            TileID.Sets.HasOutlines[type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(type);
        }
        public static void SetUpLamp(int type, bool lavaImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpLantern(int type, bool lavaImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpPiano(int type, bool lavaImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.addTile(type);
        }
        public static void SetUpSink(int type, bool lavaImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpSofa(int type, bool lavaImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpTable(int type, bool lavaImmune = false)
        {
            Main.tileSolidTop[type] = true;
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileNoAttach[type] = true;
            Main.tileTable[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpWorkBench(int type, bool lavaImmune = false)
        {
            Main.tileSolidTop[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileNoAttach[type] = true;
            Main.tileTable[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            Main.tileWaterDeath[type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        public static void SetUpTorch(int type, bool lavaImmune = false, bool waterImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileSolid[type] = false;
            Main.tileNoAttach[type] = true;
            Main.tileNoFail[type] = true;
            Main.tileWaterDeath[type] = !waterImmune;
            Main.tileLavaDeath[type] = !lavaImmune;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newTile.WaterDeath = !waterImmune;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newAlternate.WaterDeath = false;
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newAlternate.WaterDeath = false;
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
            TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
            TileObjectData.addAlternate(2);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
            TileObjectData.newAlternate.WaterDeath = false;
            TileObjectData.newAlternate.AnchorWall = true;
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(type);
        }
        public static void SetUpPlatform(int type, bool lavaImmune = false)
        {
            Main.tileLighted[type] = true;
            Main.tileFrameImportant[type] = true;
            Main.tileSolidTop[type] = true;
            Main.tileSolid[type] = true;
            Main.tileNoAttach[type] = true;
            Main.tileTable[type] = true;
            Main.tileLavaDeath[type] = !lavaImmune;
            TileID.Sets.Platforms[type] = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 27;
            TileObjectData.newTile.StyleWrapLimit = 27;
            TileObjectData.newTile.UsesCustomCanPlace = false;
            TileObjectData.newTile.LavaDeath = !lavaImmune;
            TileObjectData.addTile(type);
        }
        #endregion
        #endregion

        #region Miscellaneous Utilities
        public static void StartSandstorm()
        {
            typeof(Terraria.GameContent.Events.Sandstorm).GetMethod("StartSandstorm", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        }

        public static void StopSandstorm()
        {
            Terraria.GameContent.Events.Sandstorm.Happening = false;
        }

        public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
        {
            if (condition)
                list.Add(type);
        }
        #endregion
    }
}
