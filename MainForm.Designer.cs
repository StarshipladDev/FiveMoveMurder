using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace FiveMoveMurderFest
{
    //Code just straight stolen from
    //https://sqlchoice.azurewebsites.net/en-us/sql-server/developer-get-started/csharp/win/step/2.html
    // and http://www.java2s.com/Code/CSharp/2D-Graphics/Draganddraw.htm
    //No longer the case 05/08/2019
    //
    #region Internal Classes
    class Unit
    {
        private int accvar;
        public int acc
        {
            get { return accvar; }
            set { accvar = value; }
        }
        private bool deadvar;
        public bool dead
        {
            get { return deadvar; }
            set { deadvar = value; }
        }
        private int xposvar;
        public int xpos
        {
            get { return xposvar; }
            set { xposvar = value; }
        }
        private int yposvar;
        public int ypos
        {
            get { return yposvar; }
            set { yposvar = value; }
        }
        public Bitmap attatchedImage;
        public Unit(Bitmap image, int x, int y)
        {
            this.acc = 50;
            this.dead = false;
            this.attatchedImage = image;
            this.xpos = x;
            this.ypos = y;
        }

    }

    #endregion
    partial class MainForm
    {
        static private int maxUnits = 5;
        private Button button1;
        private Button button2;
        private TextBox textBox1;
        private Bitmap place;
        String[] moveCommands = new String[maxUnits];
        int[] moveOrder = new int[maxUnits];
        int selectedUnit = -1;
        int currentUnits = 0;
        int sQLUnit = 1;
        int actingUnit = 0;
        Unit[] units = new Unit[maxUnits];
        bool line = false;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        bool shouldPaint = false;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Componenet Generation

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();

            this.SuspendLayout();
            // 
            // button1 - Load Image/Unit
            // 
            this.button1.Location = new System.Drawing.Point(-2, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 20);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load Unit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(70, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Text = "Not RUnning";
            this.textBox1.Size = new System.Drawing.Size(157, 20);
            this.textBox1.TabIndex = 1;
            // 
            // button2- End Turn
            // 
            this.button2.Location = new System.Drawing.Point(227, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "End Turn";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Name = "Form1";
            this.Text = "FiveMoveMurderfest";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PainterForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PainterForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PainterForm_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        #region Listners
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Running";
            /* Reduntent builder coe fornon-mysql
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "";
            builder.UserID = "";              // update me
            builder.Password = "";      // update me
            builder.InitialCatalog = "";
            */

            // Connect to SQL
            Console.Write("Connecting to SQL Server ... ");
            try
            {
                string serverIp = "sql143.main-hosting.eu";
                string username = "u963414567_sld1";
                string password = "g1thub";
                string databaseName = "u963414567_rmt";
                textBox1.Text = "Attempting to connect to retreive unit ID "+sQLUnit;
                Console.Write("Attempting to connect to retreive unit ID " + sQLUnit);
                string dbConnectionString = string.Format("server={0};uid={1};pwd={2};database={3};", serverIp, username, password, databaseName);
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(dbConnectionString))
                {

                    String sql = ("SELECT ID,Name,xpos,ypos FROM peiceInfo WHERE ID=" + sQLUnit + ";");
                    Console.Write("Attempting to Open");
                    conn.Open();
                    textBox1.Text = "connected";
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
                    Console.Write("Running command");
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        String v = reader.GetString(1);
                        Console.Write("\n Setting V as name, v is "+v);
                        Console.Write("\n Testing V as null");
                        if (v == null)
                        {
                            break;
                        }
                        Console.Write("\n V not null, getting values");
                        String name = v;
                        Console.Write("\n Getting xpos");
                        int xpos = reader.GetInt32(2);
                        Console.Write("\n Getting ypos");
                        int ypos = reader.GetInt32(3);
                        Console.Write("\n Printing String with anme <"+name+">");
                        textBox1.Text = String.Format("{0},{1}\t{2},{3}", reader.GetInt32(0), "|" + name + ".png|", xpos, ypos);
                        //Remeber to add images to resource folder via add new item
                        if (currentUnits < units.Length)
                        {
                            Console.Write("\n Building unit with image location "+ "Pictures/" + name + ".png");
                            units[currentUnits] = new Unit(new Bitmap("Pictures/" + name + ".png"), xpos, ypos);
                            Graphics graphics = CreateGraphics();
                            Console.Write("\n Drawing unit");
                            graphics.DrawImage(units[currentUnits].attatchedImage, units[currentUnits].xpos, units[currentUnits].ypos);
                            graphics.Dispose();
                            currentUnits++;
                        }
                        
                    }
                    Console.Write("\n CLosing reader");
                    sQLUnit++;
                    reader.Close();
                    conn.Close();
                }
            }
            catch (Exception er)
            {
                Console.Write("\n Error:"+er.Message);
                textBox1.Text += "Error";
            }
        }
        private void PainterForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (line)
            {
                bool killed = false;
                

                int i = 0;
                while (i < currentUnits)
                {
                    if (i != selectedUnit)
                    {
                        if ((e.X > units[i].xpos && e.X < units[i].xpos + 51) && (e.Y > units[i].ypos && e.Y < units[i].ypos + 51))
                        {
                            Console.Write("\n moveCommands=" + "K" + moveCommands[actingUnit].Substring(1, 1) + i + "\n");
                            moveCommands[actingUnit] = ("K" + moveCommands[actingUnit].Substring(1,1)+ i);
                            actingUnit++;
                            killed = true;

                            i = 100000;
                            /*
                             * Kill code
                             
                            */

                        }
                        else
                        {

                            i++;
                        }
                    }
                    else
                    {
                        i++;
                    }

                }
                if (killed == false)
                {

                    Console.Write("\n moveCommands=" + moveCommands[actingUnit] + e.X.ToString("D4") + e.Y.ToString("D4") +"\n");
                    moveCommands[actingUnit] = (moveCommands[actingUnit]+e.X.ToString("D4")+e.Y.ToString("D4"));
                    actingUnit++;
                    /*MOVE CODE
                    */

                }
                line = false;
                selectedUnit = -1;
            }
            else
            {
                int i = 0;
                while (i < currentUnits)
                {
                    if ((e.X > units[i].xpos && e.X < units[i].xpos + 51) && (e.Y > units[i].ypos && e.Y < units[i].ypos + 51))
                    {
                        selectedUnit = i;
                        line = true;
                        moveCommands[actingUnit] = ("M" + i);
                        moveOrder[actingUnit] = i;
                        Console.Write("\n MoveOrder[" + actingUnit + "] Set to " + i);
                       Console.Write("\n moveCommands=" + "M" + i +"\n");
                        i = 100000;
                    }
                    else
                    {

                        i++;
                    }
                }
            }
            if (!line)
            {
                shouldPaint = true;
            }
        }

        private void PainterForm_MouseUp(object sender, MouseEventArgs e)
        {
            shouldPaint = false;
        }

        private void PainterForm_MouseMove(object sender, MouseEventArgs e)
        {
            Graphics graphics = CreateGraphics();
            Color drawColor;
            if (shouldPaint)
            {


                if (e.Button == MouseButtons.Left)
                {
                    drawColor = Color.BlueViolet;
                }
                else
                {
                    drawColor = Color.Red;
                }
                graphics.FillEllipse(new SolidBrush(drawColor), e.X, e.Y, 4, 4);

            }
            else
            {
                if (line)
                {
                    drawColor = Color.Red;
                    graphics.Clear(Color.White);
                    int i = 0;
                    while (i < currentUnits)
                    {
                        graphics.DrawImage(units[i].attatchedImage, units[i].xpos, units[i].ypos);
                        i++;

                    }
                    graphics.DrawLine(new Pen(drawColor), units[selectedUnit].xpos + 25, units[selectedUnit].ypos + 25, e.X, e.Y);

                }
            }
            graphics.Dispose();
        }
        private void button2_Click(object sender,EventArgs e)
        {


            Graphics graphics = CreateGraphics();
            graphics.Clear(Color.White);
            int i = 0;
            while (i < actingUnit){
                Console.Write("MoveOrder[" + i + "] is " + moveOrder[i]);
                if (! units[moveOrder[i]].dead)
                {
                    //If Move command
                    if (moveCommands[i].Substring(0, 1).Equals("M"))
                    {
                        Console.Write("Move Commands ["+i+"] is "+moveCommands[i]);
                        Console.Write(" Moving unit " + moveOrder[i] + " to " + moveCommands[i].Substring(2, 4) + " " + moveCommands[i].Substring(6, 4) +"\n");
                        Console.Write(" Moving unit "+moveOrder[i] +" to "+ (Int32.Parse(moveCommands[i].Substring(2, 4)) - 25) +" "+(Int32.Parse(moveCommands[i].Substring(6, 4)) - 25));
                        units[moveOrder[i]].xpos = Int32.Parse(moveCommands[i].Substring(2, 4)) - 25;
                        units[moveOrder[i]].ypos = Int32.Parse(moveCommands[i].Substring(6, 4)) - 25;
                        Console.Write("Unit deets are " +units[moveOrder[i]].xpos +","+ units[moveOrder[i]].ypos);
                    }
                    //If shoot command
                    else if (moveCommands[i].Substring(0, 1).Equals("K"))
                    {

                        Random rand = new Random();
                        if(units[moveOrder[i]].acc + rand.Next(100) > 100)
                        {

                            units[Int32.Parse(moveCommands[i].Substring(2,1))].attatchedImage = new Bitmap("Pictures/Corpse" + ((rand.Next(100) % 3) + 1) + ".png");
                            units[Int32.Parse(moveCommands[i].Substring(2, 1))].dead = true;
                        }
                        else
                        {
                            Console.Write("\n Missed the shot \n");
                        }
                    }
                    //Default error
                    else
                    {
                        Console.Write("Command error");
                    }
                }
                i++;


            }
            i = 0;
            while (i < currentUnits)
            {
                graphics.DrawImage(units[i].attatchedImage, units[i].xpos, units[i].ypos);
                i++;

            }
            
            graphics.Dispose();

            moveCommands = new String[moveCommands.Length];
            moveOrder = new int[moveOrder.Length];
            actingUnit = 0;
        }
        #endregion

    }
}

