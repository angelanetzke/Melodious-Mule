using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing.Text;

namespace MelodiousMule
{
	internal class Hero : GameObject
	{
		public enum AnimType { MOVE_N, MOVE_S, MOVE_W, MOVE_E, IDLE };
		private AnimType currentAnimation = AnimType.IDLE;
		private readonly Animation[] animations = new Animation[5];
		private readonly int SPEED = 200;
		private readonly int START_HP = 20;
		private readonly int INCREMENT_HP = 20;
		private int HP;
		private int maxHP;
		private readonly float HP_COOLDOWN = 2f;
		private float HPTimer;
		private readonly int ATTACK_RANGE = 225;
		private readonly int START_STRENGTH = 10;
		private readonly int INCREMENT_STRENGTH = 10;
		private int strength;		
		private readonly float ATTACK_COOLDOWN = .25f;
		private float attackTimer;
		private readonly ContentManager content;
		private SoundEffectInstance footstep = null;
		private readonly Random RNG = new Random();

		public Hero(float x, float y, ContentManager content) : base(x, y)
		{
			SetSize(32, 32);
			HP = START_HP;
			maxHP = START_HP;
			HPTimer = HP_COOLDOWN + 1f;
			strength = START_STRENGTH;
			attackTimer = ATTACK_COOLDOWN + 1f;
			animations[(int)AnimType.MOVE_N] = new Animation(5 * 32, 4, .2f, 32);
			animations[(int)AnimType.MOVE_S] = new Animation(0, 4, .2f, 32);
			animations[(int)AnimType.MOVE_W] = new Animation(10 * 32, 4, .2f, 32);
			animations[(int)AnimType.MOVE_E] = new Animation(10 * 32, 4, .2f, 32);
			animations[(int)AnimType.IDLE] = new Animation(4 * 32, 1, .2f, 32);
			this.content = content;
		}

		public void SetCurrentAnimation(AnimType newAnimation)
		{
			currentAnimation = newAnimation;
		}

		public void TakeDamage(int damage)
		{
			HP -= damage;
			HPTimer = 0f;
		}

		public int GetHP()
		{
			return HP;
		}

		public int GetMaxHP()
		{
			return maxHP;
		}

		public int GetStrength()
		{
			return strength;
		}		

		public void IncreaseHP()
		{
			HP += INCREMENT_HP;
			maxHP += INCREMENT_HP;
		}

		public void IncreaseStrength()
		{
			strength += INCREMENT_STRENGTH;
		}

		public bool CanAttack(List<GameObject> obstacles, float mouseX, float mouseY)
		{
			var heroX = GetPosition().X + GetSize().X / 2;
			var heroY = GetPosition().Y + GetSize().Y / 2;
			var distanceToMouse = Vector2.Distance(new Vector2(heroX, heroY), new Vector2(mouseX, mouseY));
			if (distanceToMouse > ATTACK_RANGE)
			{
				return false;
			}
			var attackRay = new Ray(
				new Vector3(heroX, heroY, 0),
				new Vector3(mouseX - heroX, mouseY - heroY, 0));
			bool canAttack = true;
			foreach (GameObject thisWall in obstacles)
			{
				float? hit = attackRay.Intersects(thisWall.GetBoundingBox());
				if (hit != null
					&& Vector2.Distance(thisWall.GetPosition(), GetPosition()) < distanceToMouse)
				{
					canAttack = false;
				}
			}
			return canAttack;
		}

		public List<Zombie> Attack(List<Zombie> enemies, float mouseX, float mouseY)
		{
			List<Zombie> defeatedEnemies = new();
			if (attackTimer > ATTACK_COOLDOWN)
			{
				foreach (Zombie thisEnemy in enemies)
				{
					if (thisEnemy.GetRectangle().Contains(mouseX, mouseY))
					{
						if (thisEnemy.TakeDamage(strength) <= 0)
						{
							defeatedEnemies.Add(thisEnemy);							
						}
						attackTimer = 0f;
					}
				}
			}
			return defeatedEnemies;
		}

		public void Reset()
		{
			HP = START_HP;
			maxHP = START_HP;
			HPTimer = HP_COOLDOWN + 1f;
			strength = START_STRENGTH;
			attackTimer = ATTACK_COOLDOWN + 1f;
		}

		public Vector2 Update(KeyboardState keyState, List<GameObject> obstacles, GameTime gameTime,
			float xTranslation, float yTranslation)
		{
			if (footstep == null)
			{
				footstep = content.Load<SoundEffect>(@"Audio\footstep").CreateInstance();
				footstep.Volume = .5f;
				footstep.IsLooped = true;
			}
			bool isHeroMoving = false;
			SetCurrentAnimation(Hero.AnimType.IDLE);
			float movement = (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED;
			if (keyState.IsKeyDown(Keys.A) && CanMoveLeft(movement, obstacles))
			{
				SetCurrentAnimation(Hero.AnimType.MOVE_W);
				SetPosition(GetPosition().X - movement, GetPosition().Y);
				xTranslation += movement;
				isHeroMoving = true;
			}
			if (keyState.IsKeyDown(Keys.D) && CanMoveRight(movement, obstacles))
			{
				SetCurrentAnimation(Hero.AnimType.MOVE_E);
				SetPosition(GetPosition().X + movement, GetPosition().Y);
				xTranslation -= movement;
				isHeroMoving = true;
			}
			if (keyState.IsKeyDown(Keys.W) && CanMoveUp(movement, obstacles))
			{
				SetCurrentAnimation(Hero.AnimType.MOVE_N);
				SetPosition(GetPosition().X, GetPosition().Y - movement);
				yTranslation += movement;
				isHeroMoving = true;
			}
			if (keyState.IsKeyDown(Keys.S) && CanMoveDown(movement, obstacles))
			{
				SetCurrentAnimation(Hero.AnimType.MOVE_S);
				SetPosition(GetPosition().X, GetPosition().Y + movement);
				yTranslation -= movement;
				isHeroMoving = true;
			}
			if (isHeroMoving)
			{
				footstep.Pitch = (float)(.5 * RNG.NextDouble() + .25);
				footstep.Play();
			}
			else
			{
				footstep.Stop();
			}
			animations[(int)currentAnimation].Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			HPTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (HPTimer >= HP_COOLDOWN && HP < maxHP)
			{
				HP++;
				HPTimer = 0f;
			}
			return new Vector2(xTranslation, yTranslation);
		}

		public override void Draw(SpriteBatch batch)
		{
			SpriteEffects effects;
			if (currentAnimation == AnimType.MOVE_W)
			{
				effects = SpriteEffects.FlipHorizontally;
			}
			else
			{
				effects = SpriteEffects.None;
			}
			batch.Draw(
				GetTexture(),
				GetRectangle(),
				animations[(int)currentAnimation].GetSourceRectangle(),
				Color.White,
				0,
				new Vector2(0, 0),
				effects,
				.6f
			);
		}

	}
}
