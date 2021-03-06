﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PLSE_Project
{
    class BodyPart // Danton // 
    {
        public Texture2D[] spriteSheets;
        public Vector2[] originVecs;
        public Vector2 position; 
        public Vector2 direction;
        public Rectangle[] sourceRect;

        public int currentActiveSprite; // will store a passed (and casted) enum to show which sprite is active, to know what to animate.
        public int[] frameAmounts; // this is the total amount of frames per spritesheet //
        public int[] sheetWidths;
        public int[] sheetHeights;
        public int[] animationCounter;
        public int[] lengthOfTimePerFrame;
        public int amountOfSheets; // amount of sheets in the array of texture 2Ds //
        public int frameLimiter;

        public bool printme = false;

        public BodyPart(ContentManager content, int amountOfSheets, int[] frameAmount, Rectangle[] sourceRect, string[] imgPath, Vector2 startingPos, int[] frameDelayTimes)
        {
            initalizeStagingArrays(amountOfSheets, frameAmount, sourceRect, imgPath);
            this.amountOfSheets = amountOfSheets;
            this.frameAmounts = new int[this.amountOfSheets];
            this.sourceRect = new Rectangle[this.amountOfSheets];
            this.lengthOfTimePerFrame = new int[this.amountOfSheets];
            this.animationCounter = new int[this.amountOfSheets];
            
            setXPos((int)startingPos.X); // this needs to come out // 
            setYPos((int)startingPos.Y);
            
            for (int counter = 0; counter < amountOfSheets; counter++)
            {
                spriteSheets[counter] = content.Load<Texture2D>(imgPath[counter]);
                sheetWidths[counter] = spriteSheets[counter].Width;
                sheetHeights[counter] = spriteSheets[counter].Height;
                this.sourceRect[counter] = sourceRect[counter];
                this.frameAmounts[counter] = frameAmount[counter];
                lengthOfTimePerFrame[counter] = frameDelayTimes[counter];
                originVecs[counter] = new Vector2(sourceRect[counter].Height / 2, sourceRect[counter].Width / 2);
                animationCounter[counter] = 1;
            }
        }

        public void draw(SpriteBatch spriteBatch, bool spriteFlipping)
        {

            if (!spriteFlipping)
                spriteBatch.Draw(spriteSheets[currentActiveSprite], position, sourceRect[currentActiveSprite], Color.White, 0, originVecs[currentActiveSprite], 1.0f, SpriteEffects.None, 1.0f);
            else
                spriteBatch.Draw(spriteSheets[currentActiveSprite], position, sourceRect[currentActiveSprite], Color.White, 0, originVecs[currentActiveSprite], 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
        }


        public void setPosition(int desiredSprite, int newX, int newY) 
        {
            position.X = newX;
            position.Y = newY;
        }
       
        public void move(int movespeed) 
        {
            position.X += movespeed;
        }
        public void unDoMove(int movespeed)
        {
            position.X -= movespeed;
        }

        public void setXPos(int x)
        {
            position.X = x;
        }
        public void setYPos(int y)
        {
            position.Y = y;
        }

        public void animate(GameTime gameTime, ContentManager content, KeyboardState keyState) // called in an update method  //
        {
            frameLimiter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (frameLimiter >= lengthOfTimePerFrame[currentActiveSprite])
            {
                frameLimiter = 0;

                sourceRect[currentActiveSprite].X = (animationCounter[currentActiveSprite] % 10) * sourceRect[currentActiveSprite].Width;
                sourceRect[currentActiveSprite].Y = (animationCounter[currentActiveSprite] / 10) * sourceRect[currentActiveSprite].Height;
                animationCounter[currentActiveSprite]++;

                if (Hero.currentActiveWeapon == (int)CurrentWeapon.Rifle && checkIfFiringTransition() && (animationCounter[currentActiveSprite] == 1 || animationCounter[currentActiveSprite] == 3) && keyState.IsKeyDown(Keys.D))
                    ProjectileManager.addBullet("rifle", content);
                else if (checkIfFiringEnergyTransition() && ((animationCounter[currentActiveSprite] == 1) || animationCounter[currentActiveSprite] == 4 || animationCounter[currentActiveSprite] == 6) && keyState.IsKeyDown(Keys.A) )
                    ProjectileManager.addBullet("energy", content);

                if (animationCounter[currentActiveSprite] >= frameAmounts[currentActiveSprite])
                    animationCounter[currentActiveSprite] = 0;
            }
        }

        private bool checkIfFiringTransition()
        {
            if (currentActiveSprite == (int)BodySpriteIndex.Shoot)
                return true;
            else if (currentActiveSprite == (int)BodySpriteIndex.CrouchShoot)
                return true;
            else if (currentActiveSprite == (int)BodySpriteIndex.ShootDown)
                return true;
            else if (currentActiveSprite == (int)BodySpriteIndex.ShootUp)
                return true;
            else
                return false;
        }
        private bool checkIfFiringEnergyTransition()
        {
            if (currentActiveSprite == (int)BodySpriteIndex.Magic)
                return true;
            else if (currentActiveSprite == (int)BodySpriteIndex.CrouchMagic)
                return true;
            else if (currentActiveSprite == (int)BodySpriteIndex.MagicDown)
                return true;
            else if (currentActiveSprite == (int)BodySpriteIndex.MagicUp)
                return true;
            else
                return false;
        }

        public void setCurrentActiveSprite(int newVal)
        {
            currentActiveSprite = newVal;
        }

        public int getCurrentActiveSprite()
        {
            return currentActiveSprite;
        }
        public void setLengthOfTimePerFrame(int newLength, int arrayPosition)
        {

        }
        public void resetAnimationValues()
        {
            animationCounter[currentActiveSprite] = 0;
            sourceRect[currentActiveSprite].Y = 0;
            sourceRect[currentActiveSprite].X = 0;
        }
        public void initalizeStagingArrays(int amountOfSheets, int[] frameAmount, Rectangle[] sourceRect, string[] imgPath)
        {
            direction = new Vector2(0, 0);
            this.frameAmounts = new int[frameAmount.Length];
            this.sourceRect = new Rectangle[sourceRect.Length];
            this.spriteSheets = new Texture2D[imgPath.Length];

            originVecs = new Vector2[amountOfSheets];
            position = new Vector2(0,0);
            sheetWidths = new int[amountOfSheets];
            sheetHeights = new int[amountOfSheets];
        }
        public void setPrintMe(bool newVal)
        {
            printme = newVal;
        }
        public void addXOffset(int offset)
        {
            position.X += offset;
        }
        public void subtXOffset(int offset)
        {
            position.X -= offset;
        }
    }
}

/*
Frame limiting logic (can be used elsewhere)
 frameLimiter += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (frameLimiter >= lengthOfTimePerFrame[currentActiveSprite])
            {
                frameLimiter = 0;
*/