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
    class Unit
    {
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
            this.attatchedImage = image;
            this.xpos = x;
            this.ypos = y;
        }

    }
    partial class MainForm
    {

        private Button button1;
        private TextBox textBox1;
        private Bitmap place;
        int selectedUnit = -1;
        int currentUnits = 0;
        Unit[] units = new Unit[5];
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(-2, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "FiveMoveMurderfest";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PainterForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PainterForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PainterForm_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Running";
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "sql143.main-hosting.eu";
            builder.UserID = "u963414567_sld1";              // update me
            builder.Password = "g1thub";      // update me
            builder.InitialCatalog = "u963414567_rmt";
            //possible value: u963414567_db1

            // Connect to SQL
            Console.Write("Connecting to SQL Server ... ");
            try
            {
                string serverIp = "";
                string username = "";
                string password = "";
                string databaseName = "";
                textBox1.Text = "Attempting to connect";
                string dbConnectionString = string.Format("server={0};uid={1};pwd={2};database={3};", serverIp, username, password, databaseName);
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(dbConnectionString))
                {

                    String sql = "SELECT ID,Name,xpos,ypos FROM peiceInfo WHERE ID=1;";
                    conn.Open();
                    textBox1.Text = "connected";
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        String name = reader.GetString(1);
                        int xpos = reader.GetInt32(2);
                        int ypos = reader.GetInt32(3);
                        textBox1.Text = String.Format("{0},{1}\t{2},{3}",reader.GetInt32(0),"|"+name+".png|",xpos,ypos);
                        if (currentUnits < units.Length)
                        {
                            units[currentUnits] = new Unit(new Bitmap("Pictures/"+name + ".png"), xpos, ypos);
                            Graphics graphics = CreateGraphics();
                            graphics.DrawImage(units[currentUnits].attatchedImage, units[currentUnits].xpos, units[currentUnits].ypos);
                            graphics.Dispose();
                            currentUnits++;
                        }
                    }
                    conn.Close();
                }
            }
            catch(Exception er)
            {
                Console.Write(er.Message);
                textBox1.Text += "Error";
            }
                
                
                
                    /* Redundent SQL code for non-mySQL
                     * REDUNDENT
                    Console.WriteLine("About to Open.");
                    connection.Open();
                    Console.WriteLine("Done.");

                    // Create a sample database
                    Console.ReadKey(true);
                    
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                String print = reader.GetInt32(0) + " " + reader.GetString(1);
                                textBox1.Text = print;
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ev)
            {
                Console.Write("\nThere was an error");
                Console.Write(ev.Message);
                textBox1.Text=(ev.Message);
            }
            */
            
        }
            private void PainterForm_MouseDown(object sender, MouseEventArgs e)
        {
            if(line)
            {
                bool killed = false;
                Graphics graphics = CreateGraphics();
                graphics.Clear(Color.White);
                
                int i = 0;
                while (i < currentUnits)
                {
                    if (i != selectedUnit)
                    {
                        if ((e.X > units[i].xpos && e.X < units[i].xpos + 51) && (e.Y > units[i].ypos && e.Y < units[i].ypos + 51))
                        {
                            Random rand = new Random();
                            units[i].attatchedImage = new Bitmap("Pictures/Corpse" + ((rand.Next(50) % 3)+1) + ".png");
                            i = 100000;
                            killed = true;

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
                    units[selectedUnit].xpos = e.X - 25;
                    units[selectedUnit].ypos = e.Y - 25;
                   
                }
                i = 0;
                while (i < currentUnits)
                {
                    graphics.DrawImage(units[i].attatchedImage, units[i].xpos, units[i].ypos);
                    i++;

                }
                line = false;
                selectedUnit = -1;
               
                graphics.Dispose();
            }
            else
            {
                int i = 0;
                while (i<currentUnits)
                {
                    if((e.X> units[i].xpos && e.X< units[i].xpos+51) && (e.Y > units[i].ypos && e.Y < units[i].ypos + 51))
                    {
                        selectedUnit = i;
                        line = true;
                        i = 100000;
                    }
                    else
                    {

                        i++;
                    }
                }
            }
            if(!line)
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
                    while (i<currentUnits)
                    {
                        graphics.DrawImage(units[i].attatchedImage, units[i].xpos, units[i].ypos);
                        i++;
                        
                    }
                    graphics.DrawLine(new Pen(drawColor), units[selectedUnit].xpos + 25, units[selectedUnit].ypos + 25, e.X, e.Y);

                }
            }
            graphics.Dispose();
        }

        #endregion

    }
}

