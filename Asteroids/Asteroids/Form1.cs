using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Asteroids
{
    public partial class Form1 : Form
    {
        private BufferedGraphicsContext context;
        private BufferedGraphics buffer;
        private System.Windows.Forms.Timer renderTimer = new Timer();
        private int counterInt = 0;
        private int oldcounterInt = 20;

        private asteroidList livingAsteroids = new asteroidList();
        private starList starfieldList = new starList();

        private Point clickPoint = new Point(0, 0);
        private Point clickPointPrevious = new Point(0, 0);

        private Random starRandom = new Random();

        private ship Avenger = new ship();
        private shotList shotsFired = new shotList();

        private static windowStuff windowInfo = new windowStuff(0,0,0,0);


        public class asteroidNode
        {
            public PointF position;
            public PointF velocity;
            public PointF[] polygon;
            public asteroidNode next;

            public asteroidNode (Point passPosition, PointF passVelocity, PointF[] passPolygon)
            {
                position = passPosition;
                velocity = passVelocity;
                polygon = passPolygon;
                next = null;
            }
                        
            
            public void DrawOne(Graphics g)
            {
                Pen pen = new Pen(Color.Gray, 2.0f);

                g.DrawPolygon(pen, polygon);
            }

            public void MoveAsteroids()
            {
                PointF oldPosition = new PointF();

                oldPosition = position;
                position.X = oldPosition.X + velocity.X;
                position.Y = oldPosition.Y + velocity.Y;

                PointF oldpoint = new PointF();

                for (int i = 0; i < polygon.Count(); i++)
                {

                    oldpoint = polygon[i];
                    polygon[i].X = oldpoint.X + velocity.X;
                    polygon[i].Y = oldpoint.Y + velocity.Y;
                }

                if (next != null)
                {
                    next.MoveAsteroids();
                }
            }
        }

        public class asteroidList
        {
            public asteroidNode asteroid;
            public int Count = 0;

            public asteroidList ()
            {
                asteroid = null;
            }

            public void AddToEnd(Point passPosition, PointF passVelocity, PointF[] passPolygon)
            {
                asteroidNode current = asteroid;
                asteroidNode nexttolast = current;
                
                while (current != null && current.next != null)
                {
                    nexttolast = current;
                    current = current.next;
                }

                if (asteroid == null)
                {
                    asteroid = new asteroidNode(passPosition, passVelocity, passPolygon);
                    Count++;
                }
                else if (current.next == null)
                {
                    current.next = new asteroidNode(passPosition, passVelocity, passPolygon);
                    Count++;
                }
            }

            public void RemoveLast()
            {
                asteroidNode current = asteroid;
                asteroidNode nextToLast = current;

                while (current != null && current.next != null)
                {
                    nextToLast = current;
                    current = current.next;
                }

                if (nextToLast == asteroid)
                {
                    asteroid = null;
                    if (Count > 0)
                    {
                        Count--;
                    }
                }
                else if (nextToLast != null)
                {
                    nextToLast.next = null;
                    if (Count > 0)
                    {                        
                        Count--;
                    }
                }
            }

            public void DrawAll(Graphics g)
            {
                asteroidNode current = asteroid;
                asteroidNode nexttolast = current;

                while (current != null)
                {
                    current.DrawOne(g);
                    
                    nexttolast = current;
                    current = current.next;
                    
                }
            }

            public void DrawOne (Graphics g)
            {
                if (asteroid != null)
                {
                    asteroid.DrawOne(g);
                }
            }

            public void MoveAsteroids ()
            {
                if (asteroid != null)
                {
                    asteroid.MoveAsteroids();
                }                
            }
        }

        public class starNode
        {
            public PointF Position;
            public Color Color;
            public int FlickerTimer;
            public int Life;
            public starNode Next;

            public starNode(PointF passPosition, Color passColor)
            {
                Position = passPosition;
                Color = passColor;
                FlickerTimer = 500;
                Life = 18;
                Next = null;
            }

            public void draw(Graphics g)
            {
                SolidBrush thisSolidBrush = null;
                

                if ((Color == Color.Gray) && (FlickerTimer == 0))
                {
                    thisSolidBrush = new SolidBrush(Color.White);
                    FlickerTimer = 15;
                    this.Color = Color.White;
                }
                else if ((Color == Color.White) && (FlickerTimer == 0))
                {
                    thisSolidBrush = new SolidBrush(Color.Gray);
                    FlickerTimer = 500;
                    Life--;
                    Color = Color.Gray;
                }

                if (FlickerTimer != 0)
                {
                    FlickerTimer--;
                    thisSolidBrush = new SolidBrush(Color);
                }
                
                Size boundingSize = new Size(2, 2);
                int pointX = (int)this.Position.X;
                int pointY = (int)this.Position.Y;
                Point intPoint = new Point(pointX, pointY);
                Rectangle boundingRectangle = new Rectangle(intPoint,  boundingSize);

                g.FillEllipse(thisSolidBrush, boundingRectangle);
            }
        }

        public class starList
        {
            public starNode Star;

            public starList ()
            {
                Star = null;
            }

            public void AddToEnd(PointF passPosition, Color passColor)
            {
                starNode current = Star;
                starNode nextToLast = current;
                
                while (current != null && current.Next != null)
                {
                    nextToLast = current;
                    current = current.Next;
                }

                if (Star == null)
                {
                    Star = new starNode(passPosition, passColor);
                }
                else if (current.Next == null)
                {
                    current.Next = new starNode(passPosition, passColor);
                }
            }

            public void RemoveLast ()
            {
                starNode current = Star;
                starNode nextToLast = current;

                while (current != null && current.Next != null)
                {
                    nextToLast = current;
                    current = current.Next;
                }

                if (current == Star)
                {
                    Star = null;
                }
                else if (nextToLast != null)
                {
                    nextToLast.Next = null;
                }
            }

            public void StarsDie()
            {
                starNode current = Star;
                starNode nexttolast = current;

                while (current != null && current.Next != null)
                {
                    if (current.Life <= 0)
                    {

                        nexttolast.Next = current.Next;
                        current = nexttolast.Next;
                        
                    }
                    else
                    {
                        nexttolast = current;
                        current = current.Next;
                    }
                }
            }

            public void DrawAll(Graphics g) 
            {
                starNode current = Star;
                starNode nexttolast = current;

                while (current != null)
                {                    
                    current.draw(g);
                    if (current.Next != null)
                    {
                        nexttolast = current;
                        current = current.Next;
                    }
                    else
                    {
                        current = null;
                    }
                }
            } 
        }

        public class windowStuff
        {
            public int windowWidth, windowHeight;
            public int windowPosX, windowPosY;

            public windowStuff(int wX, int wY, int wPX, int wPY)
            {
                windowWidth = wX;
                windowHeight = wY;
                windowPosX = wPX;
                windowPosY = wPY;
            }
        }

        public class shotNode
        {
            public Point velocity;
            public Point position;
            public shotNode next;

            public shotNode(Point vel, Point pos)
            {
                velocity = vel;
                position = pos;
                next = null;
            }

            public void draw(Graphics g)
            {
                Pen pen = new Pen(Color.Red, 2.0f);

                Point shotLineEndPoint = new Point(0,0);

                shotLineEndPoint.X = position.X + (velocity.X * 2);
                shotLineEndPoint.Y = position.Y + (velocity.Y * 2);
                g.DrawLine(pen, position, shotLineEndPoint);
            }

            public void move()
            {
                position.X = position.X + (velocity.X *2);
                position.Y = position.Y + (velocity.Y *2);
            }
        }

        public class shotList
        {
            public shotNode shot;

            public shotList()
            {
                shot = null;
            }

            public void addShot(Point vel, Point pos)
            {
                shotNode current = shot;
                shotNode nexttolast = current;

                while (current != null && current.next != null)
                {
                    nexttolast = current;
                    current = current.next;
                }
                
                if (shot == null)
                {
                    shot = new shotNode(vel, pos);
                }
                else if (current.next == null)
                {
                    current.next = new shotNode(vel, pos);
                }
            }

            public void drawAll(Graphics g)
            {
                shotNode current = shot;
                shotNode nexttolast = current;

                while (current != null)
                {
                    current.draw(g);

                    nexttolast = current;
                    current = current.next;
                }
            }

            public void moveAll()
            {
                shotNode current = shot;
                shotNode nexttolast = current;

                while (current != null)
                {
                    current.move();

                    nexttolast = current;
                    current = current.next;
                }
            }
        }

        public class ship
        {
            public Point position;
            public Point velocity;
            public Point[] shipShape;
            public int throttle;

            public ship()
            {
                position.X = 600;
                position.Y = 350;

                velocity = new Point(0, 0);

                throttle = 1;

                Point a = new Point(200,200);
                Point b = new Point(215,200);
                Point c = new Point(200,215);

                shipShape = new Point[] {
                                    a,
                                    b,
                                    c                                    
                                     };

            }

            public void move()
            {
                position.X += velocity.X;
                position.Y += velocity.Y;

                if (position.X >= windowInfo.windowWidth)
                {
                    position.X = 0;
                }
                else if (position.Y >= windowInfo.windowHeight)
                {
                    position.Y = 0;
                }
                else if (position.X <= 0)
                {
                    position.X = windowInfo.windowWidth;
                }
                else if (position.Y <= 0)
                {
                    position.Y = windowInfo.windowHeight;
                }

            }

            public void draw(Graphics g)
            {
                shipShape[0].X = position.X;
                shipShape[0].Y = position.Y;

                if (velocity.X < 0)
                {
                    if (velocity.Y < 0)
                    {
                        shipShape[1].X = position.X + 14;
                        shipShape[1].Y = position.Y;

                        shipShape[2].X = position.X;
                        shipShape[2].Y = position.Y + 14;
                    }
                    else if (velocity.Y > 0)
                    {
                        shipShape[1].X = position.X;
                        shipShape[1].Y = position.Y - 14;

                        shipShape[2].X = position.X + 14;
                        shipShape[2].Y = position.Y;
                    }
                    else if (velocity.Y == 0)
                    {
                        shipShape[1].X = position.X + 10;
                        shipShape[1].Y = position.Y - 10;

                        shipShape[2].X = position.X + 10;
                        shipShape[2].Y = position.Y + 10;
                    }
                }
                else if (velocity.X > 0)
                {
                    if (velocity.Y < 0)
                    {
                        shipShape[1].X = position.X - 14;
                        shipShape[1].Y = position.Y;

                        shipShape[2].X = position.X;
                        shipShape[2].Y = position.Y + 14;
                    }
                    else if (velocity.Y > 0)
                    {
                        shipShape[1].X = position.X;
                        shipShape[1].Y = position.Y - 14;

                        shipShape[2].X = position.X - 14;
                        shipShape[2].Y = position.Y;
                    }
                    else if (velocity.Y == 0)
                    {
                        shipShape[1].X = position.X - 10;
                        shipShape[1].Y = position.Y + 10;

                        shipShape[2].X = position.X - 10;
                        shipShape[2].Y = position.Y - 10;
                    }
                }
                else if (velocity.X == 0)
                {
                    if (velocity.Y < 0)
                    {
                        shipShape[1].X = position.X - 10;
                        shipShape[1].Y = position.Y + 10;

                        shipShape[2].X = position.X + 10;
                        shipShape[2].Y = position.Y + 10;
                    }
                    else if (velocity.Y > 0)
                    {
                        shipShape[1].X = position.X + 10;
                        shipShape[1].Y = position.Y - 10;

                        shipShape[2].X = position.X - 10;
                        shipShape[2].Y = position.Y - 10;
                    }
                    else if (velocity.Y == 0)
                    {
                        shipShape[1].X = position.X - 10;
                        shipShape[1].Y = position.Y + 10;

                        shipShape[2].X = position.X - 10;
                        shipShape[2].Y = position.Y - 10;
                    }
                }

               

                Pen pen = new Pen(Color.Blue, 1);

                g.DrawPolygon(pen, shipShape);
            }
        }       

        public Form1()
        {
            InitializeComponent();

            renderTimer.Tick += new EventHandler(RenderTimerEventHandler);
            renderTimer.Interval = 24;
            renderTimer.Start();

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            context = BufferedGraphicsManager.Current;

            windowInfo.windowWidth = this.Width;
            windowInfo.windowHeight = this.Height;
            windowInfo.windowPosX = this.Location.X;
            windowInfo.windowPosY = this.Location.Y;
            
            GameLoop();
            UpdateBuffer();
        }

        private void RenderTimerEventHandler(Object o, EventArgs e)
        {
            counterInt ++;
            GameLoop();
            UpdateBuffer();
            this.Refresh();
        }

        private void GameLoop()
        {
            ShipProcessing();
            shotsFired.moveAll();
            AddToStarField();
            livingAsteroids.MoveAsteroids();
        }

        private void UpdateBuffer()
        {
            context.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            buffer = context.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));

            DrawToBuffer(buffer.Graphics);
        }

        private void DrawToBuffer(Graphics g)
        {
            //Pen pen = new Pen(Color.Red, 1.0f);

            //g.DrawLine(pen, clickPointPrevious, clickPoint);

            g.DrawString(counterInt.ToString(), new Font("Arial", 10), System.Drawing.Brushes.Black, new RectangleF(30,30,100,50));
            this.Text = counterInt.ToString() + " | Asteroid count:" + livingAsteroids.Count.ToString();

            Avenger.draw(g);
            shotsFired.drawAll(g);
            starfieldList.StarsDie();
            starfieldList.DrawAll(g);
            livingAsteroids.DrawAll(g);
        }

        private void CreateAsteroid(float posX, float posY, float size)
        {       
            PointF point0 = new PointF(posX + size, posY);
            PointF point1 = new PointF(posX + size, posY + size);
            PointF point2 = new PointF(posX, posY + (size * 2));
            PointF point3 = new PointF(posX - size, posY + size);
            PointF point4 = new PointF(posX - size, posY);
            PointF point5 = new PointF(posX - size, posY - size);
            PointF point6 = new PointF(posX, posY - (size * 2));

            PointF [] polygonPoints = {
                                         point0,
                                         point1,
                                         point2,
                                         point3,
                                         point4,
                                         point5,
                                         point6
                                     };
            float a;
            float b;
            
            a = (float)numericUpDown1.Value;
            b = (float)numericUpDown2.Value;
            PointF velocity = new PointF(a, b);

            livingAsteroids.AddToEnd(clickPoint, velocity, polygonPoints);            
        }

        private void AddToStarField()
        {
            int newstarx, newstary = 0;

            if (counterInt - oldcounterInt > 5)
            {
                
                oldcounterInt = counterInt;

                newstarx = starRandom.Next(0, this.Width);
                newstary = starRandom.Next(0, this.Height);
                Point newStarPoint = new Point(newstarx, newstary);

                starfieldList.AddToEnd(newStarPoint, Color.Gray);
            }
        }

        private void ShipProcessing()
        {
            Avenger.move();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            buffer.Render(e.Graphics);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Click(object sender, EventArgs e)
        {
            clickPointPrevious = clickPoint;    
            clickPoint = this.PointToClient(Cursor.Position);

            Random randSize = new Random();
            float randSizeF;
                
            randSizeF = (float)randSize.Next(0, 20);
            
            CreateAsteroid(clickPoint.X, clickPoint.Y, randSizeF);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            livingAsteroids.RemoveLast();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'a')
            {
                if (Avenger.velocity.X > -3)
                {
                    Avenger.velocity.X--;
                }                
            }
            else if (e.KeyChar == 'w')
            {
                if (Avenger.velocity.Y > -3)
                {
                    Avenger.velocity.Y--;
                }                
            }
            else if (e.KeyChar == 's')
            {
                if (Avenger.velocity.Y < 3)
                {
                    Avenger.velocity.Y++;
                }                
            }
            else if (e.KeyChar == 'd')
            {
                if (Avenger.velocity.X < 3)
                {
                    Avenger.velocity.X++;
                }                
            }
            else if (e.KeyChar == 'f')
            {
                shotsFired.addShot(Avenger.velocity, Avenger.position);
            }
            e.Handled = true;
        }

        

    }
}
