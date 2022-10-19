using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MelodiousMule
{
	internal class Zombie : GameObject
	{
		public enum Difficulty { EASY, MEDIUM, HARD };
		private readonly Difficulty difficulty;
		private readonly int[] SPEED = new int[] { 50, 100, 150 };
		private readonly int[] VISION = new int[] { 200, 250, 300 };
		private readonly int[] DAMAGE = new int[] { 1, 10, 25 };
		private readonly int[] MAX_HP = new int[] { 50, 75, 100};
		private int currentHP;
		private readonly float ATTACK_COOLDOWN = 1.0f;
		private float attackTimer;
		private readonly float DAMAGE_EFFECT_COOLDOWN = .25f;
		private float damageEffectTimer;
		public enum AnimType { MOVE_N, MOVE_S, MOVE_W, MOVE_E, IDLE };
		private AnimType currentAnimation = AnimType.IDLE;
		private Animation[] animations = new Animation[5];
		public Zombie(float x, float y, Difficulty difficulty) : base(x, y)
		{
			this.difficulty = difficulty;
			SetSize(30, 35);
			currentHP = MAX_HP[(int)difficulty];
			attackTimer = ATTACK_COOLDOWN + 1f;
			damageEffectTimer = DAMAGE_EFFECT_COOLDOWN + 1f;
			animations[(int)AnimType.MOVE_N] = new Animation(
				9 * (int)Math.Round(GetSize().X), 4, .2f, (int)Math.Round(GetSize().X));
			animations[(int)AnimType.MOVE_S] = new Animation(
				(int)Math.Round(GetSize().X), 4, .2f, (int)Math.Round(GetSize().X));
			animations[(int)AnimType.MOVE_W] = new Animation(
				13 * (int)Math.Round(GetSize().X), 4, .2f, (int)Math.Round(GetSize().X));
			animations[(int)AnimType.MOVE_E] = new Animation(
				5 * (int)Math.Round(GetSize().X), 4, .2f, (int)Math.Round(GetSize().X));
			animations[(int)AnimType.IDLE] = new Animation(0, 1, .2f, (int)Math.Round(GetSize().X));
		}

		public void Update(Hero target, List<GameObject> obstacles, GameTime gameTime)
		{
			var movement = (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED[(int)difficulty];
			SetCurrentAnimation(AnimType.IDLE);
			if (Vector2.Distance(target.GetPosition(), GetPosition()) < VISION[(int)difficulty])
			{
				var xDistance = 0;
				var yDistance = 0;
				var xAnimation = AnimType.IDLE;
				var yAnimation = AnimType.IDLE;
				if (target.GetPosition().X < GetPosition().X && CanMoveLeft(movement, obstacles))
				{
					xAnimation = AnimType.MOVE_W;
					xDistance = (int)(GetPosition().X - target.GetPosition().X);
					SetPosition(GetPosition().X - movement, GetPosition().Y);
				}
				if (target.GetPosition().X > GetPosition().X && CanMoveRight(movement, obstacles))
				{
					xAnimation = AnimType.MOVE_E;
					xDistance = (int)(target.GetPosition().X - GetPosition().X);
					SetPosition(GetPosition().X + movement, GetPosition().Y);
				}
				if (target.GetPosition().Y < GetPosition().Y && CanMoveUp(movement, obstacles))
				{
					yAnimation = AnimType.MOVE_N;
					yDistance = (int)(GetPosition().Y - target.GetPosition().Y);
					SetPosition(GetPosition().X, GetPosition().Y - movement);
				}
				if (target.GetPosition().Y > GetPosition().Y && CanMoveDown(movement, obstacles))
				{
					yAnimation = AnimType.MOVE_S;
					yDistance = (int)(target.GetPosition().Y - GetPosition().Y);
					SetPosition(GetPosition().X, GetPosition().Y + movement);
				}
				if (xDistance > yDistance)
				{
					SetCurrentAnimation(xAnimation);
				}
				else if (yDistance > xDistance)
				{
					SetCurrentAnimation(yAnimation);
				}
			}
			animations[(int)currentAnimation].Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (GetRectangle().Intersects(target.GetRectangle()) && attackTimer >= ATTACK_COOLDOWN)
			{
				attackTimer = 0f;
				target.TakeDamage(DAMAGE[(int)difficulty]);
			}
			damageEffectTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
		}

		public void SetCurrentAnimation(AnimType newAnimation)
		{
			currentAnimation = newAnimation;
		}

		public int TakeDamage(int damage)
		{
			currentHP -= damage;
			damageEffectTimer = 0f;
			return currentHP;			
		}

		public override void Draw(SpriteBatch batch)
		{
			Color drawColor = Color.White; 
			if (damageEffectTimer < DAMAGE_EFFECT_COOLDOWN)
			{
				drawColor = Color.Red;
			}
			batch.Draw(
				GetTexture(),
				GetRectangle(),
				animations[(int)currentAnimation].GetSourceRectangle(),
				drawColor,
				0,
				new Vector2(0, 0),
				SpriteEffects.None,
				.6f
			);
		}

	}
}
