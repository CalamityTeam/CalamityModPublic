using CalamityMod.Items.DraedonMisc;
using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.TileEntities
{
	public class TEChargingStation : ModTileEntity
	{
		#region Spark Class
		public class Spark
		{
			public int Steps;
			public Vector2 Start;
			public Vector2 End;
			public Vector2 CurrentPosition;
			public Vector2 Velocity;
			public Vector2[] OldPositions = new Vector2[15];
			public const float SparkSpeed = 3.25f;
			public void Update()
			{
				if (Steps > 12)
				{
					Velocity = Vector2.Lerp(Velocity, Vector2.Normalize(End - CurrentPosition) * SparkSpeed, 0.15f);
				}
				else if (Main.rand.NextBool(5))
				{
					Velocity = Vector2.Normalize(End - CurrentPosition).RotatedByRandom(0.3f) * SparkSpeed;
				}

				CurrentPosition += Velocity;
				for (int i = OldPositions.Length - 1; i > 0; i--)
				{
					OldPositions[i] = OldPositions[i - 1];
				}
				OldPositions[0] = CurrentPosition;
				Steps++;
			}
			public bool ShouldDestroy()
			{
				for (int i = 0; i < OldPositions.Length; i++)
				{
					if (Vector2.Distance(End, OldPositions[i]) > 10f)
						return false;
				}
				return true;
			}
		}
		#endregion

		#region Fields
		public int Time;
		public int Charge;
		public int ChargeMax;
		public int ActiveTimer;
		public int DepositWithdrawCooldown;
		public Item FuelItem = new Item();
		public Item ItemBeingCharged = new Item();
		public Spark[] Sparks = new Spark[9];
		public const int TotalSparkUpdatesPerFrame = 3;
		public const int ActiveTimerMax = 30;

		#endregion

		public void AttemptToCreateNewSpark()
		{
			int i = 0;
			while (Sparks[i] != null)
			{
				i++;
				if (i >= Sparks.Length)
					break;
			}
			if (i < Sparks.Length)
			{
				Sparks[i] = new Spark()
				{
					CurrentPosition = FuelItem.position,
					Start = FuelItem.position + Main.rand.NextBool(2).ToDirectionInt() * Vector2.UnitX * 32f,
					End = ItemBeingCharged.position + Utils.RandomVector2(Main.rand, -16f, 16f) * new Vector2(1f, 0.2f),
					Velocity = Vector2.Normalize(ItemBeingCharged.position - FuelItem.position).RotatedBy(Main.rand.NextFloat(0.9f, 1.4f) * Main.rand.NextBool(2).ToDirectionInt()) * Spark.SparkSpeed
				};
			}
		}
		public void DrawAllSparks(SpriteBatch spriteBatch)
		{
			Texture2D tinySparkTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/RedLightningTexture");
			for (int i = 0; i < Sparks.Length; i++)
			{
				if (Sparks[i] == null)
					continue;
				Spark spark = Sparks[i];

				for (int j = 0; j < TotalSparkUpdatesPerFrame; j++)
				{
					spark.Update();
				}

				DelegateMethods.c_1 = Color.White;
				DelegateMethods.f_1 = 1f;
				for (int j = 0; j < spark.OldPositions.Length - 1; j++) 
				{
					float scale = (float)Math.Sin(j / (float)spark.OldPositions.Length * MathHelper.Pi) * 2f;
					scale = MathHelper.Clamp(scale, 0.25f, 2f) * 0.1f;
					if (spark.OldPositions[j] != Vector2.Zero &&
						spark.OldPositions[j + 1] != Vector2.Zero)
					{
						Utils.DrawLaser(spriteBatch, tinySparkTexture, spark.OldPositions[j] - Main.screenPosition, spark.OldPositions[j + 1] - Main.screenPosition, new Vector2(scale), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
					}
				}
				if (spark.ShouldDestroy())
				{
					Sparks[i] = null;
				}
			}
		}

		public void ClientToServerSync()
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				var netMessage = CalamityMod.Instance.GetPacket();
				netMessage.Write((byte)CalamityModMessageType.DraedonChargerSync);
				netMessage.Write(ID);
				netMessage.Write(FuelItem.type);
				netMessage.Write(FuelItem.stack);
				netMessage.WriteVector2(FuelItem.position);
				netMessage.Write(ItemBeingCharged.type);
				netMessage.Write(ItemBeingCharged.stack);
				netMessage.Write(ItemBeingCharged.prefix);
				netMessage.WriteVector2(ItemBeingCharged.position);
				netMessage.Write(Charge);
				netMessage.Write(ChargeMax);
				netMessage.Write(ActiveTimer);
				netMessage.Write(DepositWithdrawCooldown);
				netMessage.Send();
			}
		}

		public override bool ValidTile(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			return tile.active() && tile.type == ModContent.TileType<DraedonItemCharger>() && tile.frameX == 0 && tile.frameY == 0;
		}
		public override void Update()
		{
			if (FuelItem is null || FuelItem.type != ModContent.ItemType<PowerCell>())
			{
				FuelItem = new Item();
				FuelItem.SetDefaults(ModContent.ItemType<PowerCell>());
				FuelItem.stack = 0;
				ClientToServerSync();
			}
			if (ItemBeingCharged is null)
			{
				ItemBeingCharged = new Item();
				ItemBeingCharged.stack = 0;
				ClientToServerSync();
			}
			FuelItem.favorited = false;
			ItemBeingCharged.favorited = false;
			Time++;
			if (Time % 60 == 0)
			{
				ClientToServerSync();
			}
			if (FuelItem.stack > 0 &&
				ItemBeingCharged.stack > 0 &&
				Charge < ChargeMax)
			{
				if (Time % 25 == 0)
				{
					for (int i = 0; i < Main.rand.Next(1, 3 + 1); i++)
					{
						AttemptToCreateNewSpark();
					}
					Main.PlaySound(SoundID.Item15, FuelItem.position);
					Charge++;
					FuelItem.stack--;
					ClientToServerSync();
				}
				if (ActiveTimer < ActiveTimerMax)
					ActiveTimer++;
			}
			else if (ActiveTimer > 0) ActiveTimer--;

			if (DepositWithdrawCooldown > 0)
				DepositWithdrawCooldown--;
		}
		public override void OnKill()
		{
			if (Main.LocalPlayer.Calamity().CurrentlyViewedCharger == this)
				Main.LocalPlayer.Calamity().CurrentlyViewedCharger = null;
			base.OnKill();
		}
		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				NetMessage.SendTileSquare(Main.myPlayer, i, j, 5);
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
				return -1;
			}
			return Place(i, j);
		}
		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			writer.Write(ID);
			writer.Write(FuelItem.type);
			writer.Write(FuelItem.stack);
			writer.WriteVector2(FuelItem.position);
			writer.Write(ItemBeingCharged.type);
			writer.Write(ItemBeingCharged.stack);
			writer.Write(ItemBeingCharged.prefix);
			writer.WriteVector2(ItemBeingCharged.position);
			writer.Write(Charge);
			writer.Write(ChargeMax);
			writer.Write(ActiveTimer);
		}
		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			ID = reader.ReadInt32();
			FuelItem = new Item
			{
				type = reader.ReadInt32()
			};
			FuelItem.SetDefaults(FuelItem.type);
			FuelItem.stack = reader.ReadInt32();
			FuelItem.position = reader.ReadVector2();
			ItemBeingCharged.SetDefaults(reader.ReadInt32());
			ItemBeingCharged.stack = reader.ReadInt32();
			ItemBeingCharged.prefix = reader.ReadByte();
			ItemBeingCharged.position = reader.ReadVector2();
			Charge = reader.ReadInt32();
			ChargeMax = reader.ReadInt32();
			ActiveTimer = reader.ReadInt32();
		}
		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound
			{
				["ID"] = ID, // Don't ask me why, but the ID doesn't get saved in MP otherwise.
				["StackFuel"] = FuelItem.stack,
				["PrefixFuel"] = FuelItem.prefix,
				["NetIDFuel"] = FuelItem.active && FuelItem.stack > 0 ? FuelItem.netID : 0,
				["FuelPosition"] = FuelItem.position,

				["StackNotFuel"] = ItemBeingCharged.stack,
				["PrefixNotFuel"] = ItemBeingCharged.prefix,
				["PositionNotFuel"] = ItemBeingCharged.position,
				["Charge"] = Charge,
				["ChargeMax"] = ChargeMax,
				["NetIDNotFuel"] = ItemBeingCharged.active && FuelItem.stack > 0 ? FuelItem.netID : 0
			};
			CalamityUtils.SaveModItem(tag, FuelItem, 0);
			CalamityUtils.SaveModItem(tag, ItemBeingCharged, 1);
			return tag;
		}
		public override void Load(TagCompound tag)
		{
			ID = tag.GetInt("ID");
			FuelItem = CalamityUtils.LoadModItem(tag, 0);
			FuelItem.stack = tag.GetInt("StackFuel");
			FuelItem.prefix = tag.GetByte("PrefixFuel");
			FuelItem.netID = tag.GetInt("NetIDFuel");
			FuelItem.position = tag.Get<Vector2>("FuelPosition");

			ItemBeingCharged = CalamityUtils.LoadModItem(tag, 1);
			ItemBeingCharged.stack = tag.GetInt("StackNotFuel");
			ItemBeingCharged.prefix = tag.GetByte("PrefixNotFuel");
			ItemBeingCharged.position = tag.Get<Vector2>("PositionNotFuel");
			if (ItemBeingCharged.type > ItemID.Count)
			{
				Charge = tag.GetInt("Charge");
				ChargeMax = tag.GetInt("ChargeMax");
			}
			ItemBeingCharged.netID = tag.GetInt("NetIDNotFuel");
		}
	}
}
