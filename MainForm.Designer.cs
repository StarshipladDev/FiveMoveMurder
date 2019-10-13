using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Media;

/*
 * Documentation types:
<c>	Set text in a code-like font
<code>	Set one or more lines of source code or program output
<example>	Indicate an example
<exception>	Identifies the exceptions a method can throw
<include>	Includes XML from an external file
<list>	Create a list or table
<para>	Permit structure to be added to text
<param>	Describe a parameter for a method or constructor
<paramref>	Identify that a word is a parameter name
<permission>	Document the security accessibility of a member
<remark>	Describe additional information about a type
<returns>	Describe the return value of a method
<see>	Specify a link
<seealso>	Generate a See Also entry
<summary>	Describe a type or a member of a type
<value>	Describe a property
<typeparam>		Describe a generic type parameter
<typeparamref>		Identify that a word is a type parameter name
 */
namespace FiveMoveMurderFest
{
    //Code just straight stolen from
    //https://sqlchoice.azurewebsites.net/en-us/sql-server/developer-get-started/csharp/win/step/2.html
    // and http://www.java2s.com/Code/CSharp/2D-Graphics/Draganddraw.htm
    //No longer the case 05/08/2019
    //
    //Kill Move - 'K' | Shooter index | shot unit index
    //Move Commnad - 'M| Moving unit's index | 4 digits, leading zeros, x move co-ordinate | 4 digits, leading zeros, Y move co-ordinate
    /**
 * <summary> A Unit is a dynamic actionable object dispalyed in the gameworld. It represetns a character or some other 'alive' thing with agency.<para />
 * Unit has cooridinates and individual stats that can be called for tests. A units' *Image* can be changed and redrawn in the enviroment, and is the representation fo the unit</summary>
 * 
 * 
 * 
 */
    #region Internal Classes
    class Unit
    {
        private int accvar;
        public int acc
        {
            get { return accvar; }
            set { accvar = value; }
        }
        private bool actedVar;
        public bool acted
        {
            get { return actedVar; }
            set { actedVar = value; }
        }
        private int initiativeVar;
        public int init
        {
            get { return initiativeVar; }
            set { initiativeVar = value; }
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
        private int teamvar;
        public int team
        {
            get { return teamvar; }
            set { teamvar = value; }
        }
        private int yposvar;
        public int ypos
        {
            get { return yposvar; }
            set { yposvar = value; }
        }
        public Bitmap attatchedImage;
        /**
         * <summary> Initializes a new instance of the <see cref="Unit">Unit</see>   class, alive,with an acc of 50 and unacted. </summary>
         * <param name="image"> Image is the bitmap image that the unit will start off dispalyed as</param>
         * <param name="x">The X-coordiante the unit will start in</param>
         * <param name="y"> The Y-coordiante the unit will start in</param>
         */
        public Unit(Bitmap image, int x, int y,int team,int init)
        {
            this.acc = 50;
            this.dead = false;
            this.acted = false;
            this.attatchedImage = image;
            this.xpos = x;
            this.ypos = y;
            this.team = team;
            this.init = init; ;
        }

    }

    #endregion
    /**
     * <summary>MainForm is the opening form of Five Move Murderfest, and holds all global variables and game logic </summary>
     */
    partial class MainForm
    {

        private int windowHeight = 450;
        private int windowLength = 800;
        static private int maxUnits = 10;
        //Interactive components
        private Button button1;
        private Button button2;
        private TextBox textBox1;
        private ToolBar toolBar;
        ToolBarButton toolButton1 = new ToolBarButton();
        ToolBarButton toolButton2 = new ToolBarButton();
        ToolBarButton toolButton3 = new ToolBarButton();
        //End Of Interactive components
        static Random rand = new Random();
        String[] moveCommands = new String[maxUnits];
        int[] moveOrder = new int[maxUnits];
        int[] lineDraw = new int[maxUnits * 4];
        int[] audioQueue = new int[maxUnits];
        int selectedUnit = -1;
        //Terrain- gridTypeDownRight MUST be a square number
        static int[] gridTypeDownRight= new int[9] {rand.Next(6)-3, rand.Next(6) - 3, rand.Next(6) - 3,rand.Next(6) - 3, rand.Next(6) - 3, rand.Next(6) - 3, rand.Next(6) - 3, rand.Next(6) - 3, rand.Next(6) - 3};
        static Color[] terrainColors = new Color[gridTypeDownRight.Length];
        int ammount = (int)Math.Sqrt(gridTypeDownRight.Length);
        //End of Terrain
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
        private void SetTerrain()
        {
            int i = 0;
            while (i<gridTypeDownRight.Length)
            {
                if (gridTypeDownRight[i] == 1)
                {
                    terrainColors[i] = Color.FromArgb(255, (Color.Brown.R + rand.Next(20) - 10) % 255, (Color.Brown.G + rand.Next(20) - 10) % 255, (Color.Brown.B + rand.Next(20) - 10) % 255);
                }
                else if (gridTypeDownRight[i] == 2)
                {
                    terrainColors[i] = Color.FromArgb(255, (Color.Gray.R + rand.Next(20) - 10) % 255, (Color.Gray.G + rand.Next(20) - 10) % 255, (Color.Gray.B + rand.Next(20) - 10) % 255);
                }
            i++;
            }
        }
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            /*
             *   SETIP TERRAIN COLORS
             * 
             */
            SetTerrain();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            //
            //INITIALIZE INTERACTIVE COMPONENTS - Must be decleared at start of MainForm
            //

            this.toolBar = new ToolBar();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            //
            //ToolBar and contaiend buttons
            //
            this.toolBar.Size = new System.Drawing.Size(windowLength, 10);
            this.toolButton1.Text = "New Hero Unit";
            this.toolButton2.Text = "New Bad Unit";
            this.toolButton3.Text = "Choose Image For Latest Unit";
            this.toolBar.Buttons.Add(toolButton1);
            this.toolBar.Buttons.Add(toolButton2);
            this.toolBar.Buttons.Add(toolButton3);
            this.toolBar.ButtonClick += new ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
            // 
            // button1 - Load Image/Unit
            // 
            this.button1.Location = new System.Drawing.Point(0, -2 + this.toolBar.Height);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load Unit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(75,0+this.toolBar.Height);
            this.textBox1.Name = "textBox1";
            this.textBox1.Text = "Not RUnning";
            this.textBox1.Size = new System.Drawing.Size(180, 23);
            this.textBox1.TabIndex = 1;
            // 
            // button2- End Turn
            // 
            this.button2.Location = new System.Drawing.Point(255,-2+ this.toolBar.Height);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "End Turn";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.Icon= new Icon("Pictures/FMMF_icon.ico");
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(windowLength,windowHeight+this.toolBar.Height);
            this.Controls.Add(toolBar);
            Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Name = "Form1";
            this.Text = "FiveMoveMurderfest";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PainterForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PainterForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PainterForm_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Shown += new System.EventHandler(this.MainForm_Load); 
        }
        #endregion
        /**
         * <summary> Utility method to sort units by initiative values, based on command strings passed
         * </summary>
         * 
         * <returns>A sorted String of move commands <see cref="actionEnemy"></see></returns>
         * <param name="moveCommandsTemp">The unsorted string of Move commands <see cref="actionEnemy"></see></param>
         * 
         */
        private string[] sortMoves(String[] moveCommandsTemp)
        {
            int i = 0;
            while (i< moveCommandsTemp.Length && moveCommandsTemp[i] != null)
            {
                Console.Write("\n sorting " + i +" with ");
                int f = i;
                while (f < moveCommandsTemp.Length-1 && moveCommandsTemp[f+1] != null)
                {
                    Console.Write(f);
                    if (units[Int32.Parse(moveCommandsTemp[f].Substring(1,1))].init > units[Int32.Parse(moveCommandsTemp[f+1].Substring(1, 1))].init)
                    {
                        String temp = moveCommandsTemp[f+1];
                        moveCommandsTemp[f + 1] = moveCommandsTemp[f];
                        moveCommandsTemp[f] = temp;
                    }
                    f++;
                }
                i++;
            }
            return moveCommandsTemp;
        }
        /**
    * <summary>Draws terrain once window of applciation is shown subscribed to the 'shown' method group </summary>
    */
        private void MainForm_Load(object sender, EventArgs e)
        {
            Application.DoEvents();
            using (Graphics g = CreateGraphics())
            {

                DrawTerrain(g, ammount);
                g.Dispose();
            }
    

            }
#region Listners
/**
 * <summary>button1_Click deals with the SQL loading of several variables from an SQL database. It then dispalys output text to a console and textbox and draws </summary>
 */
private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Running";

            /* Reduntent builder code for non-mysql
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
                string serverIp = "";
                string username = "";
                string password = "";
                string databaseName = "";
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
                            units[currentUnits] = new Unit(new Bitmap("Pictures/" + name + ".png"), xpos, ypos+this.toolBar.Height,1,rand.Next(3)+1);
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

            Graphics g = this.CreateGraphics();
            DrawTerrain(g, ammount);
            int i = 0;
            while (i < currentUnits)
            {

                DrawUnits(g, i);
                i++;
            }
            g.Dispose();
        }
        /**
         * <summary>Handles any mouse down events performed on MainForm<para />
         * If a unit is clicked or already selected, it handles the logic for adding a command string to be run in order when <see cref="button2_Click"></see> is clicked</summary>
         */
        private void PainterForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (line)
            {
                bool killed = false;
                lineDraw[actingUnit * 4] =units[selectedUnit].xpos + 25;
                lineDraw[(actingUnit * 4)+1] = units[selectedUnit].ypos + 25;
                lineDraw[(actingUnit * 4) + 2] = e.X;
                lineDraw[(actingUnit * 4) + 3] = e.Y;
                Console.Write("\n Set Values as " + lineDraw[actingUnit * 4] + " " + lineDraw[(actingUnit * 4) + 1] + " " + lineDraw[(actingUnit * 4) + 2] + " " + lineDraw[(actingUnit * 4) + 3]);
                int i = 0;
                while (i < currentUnits)
                {
                    
                    if (i != selectedUnit)
                    {

                        /*
                         * Kill code

                        */
                        if ((e.X > units[i].xpos && e.X < units[i].xpos + 51) && (e.Y> units[i].ypos && e.Y < units[i].ypos + 51))
                        {
                            Console.Write("\n moveCommands=" + "K" + moveCommands[actingUnit].Substring(1, 1) + i + "\n");
                            moveCommands[actingUnit] = ("K" + moveCommands[actingUnit].Substring(1,1)+ i);
                            killed = true;
                            i = 100000;

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
                    
                    /*MOVE CODE
                    */

                }
                actingUnit++;
                line = false;
                selectedUnit = -1;

            }
            else
            {
                int i = 0;
                while (i < currentUnits)
                {
                    Console.Write("\n \n Mouse x,y " + e.X + " " + e.Y + " , unit coords " + units[i].xpos + " " + units[i].ypos );
                    if ((e.X > units[i].xpos && e.X < units[i].xpos + 51) && (e.Y > units[i].ypos && e.Y < units[i].ypos + 51))
                    {
                        Console.Write("Unit "+i+ " is selected and acted is " +units[i].acted+"\n \n");
                        if (units[i].acted == false && units[i].team==1)
                        {
                            selectedUnit = i;
                            Console.Write("Unit " + i + " acted " + units[i].acted);
                            units[i].acted = true;
                            Console.Write("Unit " + i + " changed, acted is " + units[i].acted + "\n \n");
                            line = true;
                            moveCommands[actingUnit] = ("M" + i);
                            moveOrder[actingUnit] = i;
                            Console.Write("\n MoveOrder[" + actingUnit + "] Set to " + i);
                            Console.Write("\n moveCommands=" + "M" + i + "\n");
                            
                        }
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
        /**
         * <summary>Utility method to draw all terrain as their respective colros based on terrain type</summary>
         */
        private void DrawTerrain(Graphics graphics,int ammount)
        {
            int i = 0;
            /* 
            *DRAW TERRAIN 
            * 
            */
            //Ammount in a row of grid
            while (i < ammount)
            {
                int f = 0;
                while (f < ammount)
                {
                    if (gridTypeDownRight[(i * ammount) + f] >0)
                    {
                        SolidBrush brush = null;
                        if (gridTypeDownRight[(i * ammount) + f] == 1)
                        {
                            brush = new SolidBrush(terrainColors[(i * ammount) + f]);
                        }
                        else if (gridTypeDownRight[(i * ammount) + f] == 2)
                        {
                            brush = new SolidBrush(terrainColors[(i * ammount) + f]);
                        }
                        graphics.FillRectangle(brush, new RectangleF(i * (windowLength / ammount), f * (windowHeight / ammount) + this.toolBar.Height, windowLength / ammount, windowHeight / ammount));
                    }
                    f++;
                }
                i++;
            }
            /*
             * END OF DRAW TERRAIN
             * 
             */
        }
        /**
         * <summary> Utility method to iterativly call all units  currently initialized</summary>
         */
        private void DrawUnits(Graphics graphics, int i)
        {
            graphics.DrawImage(units[i].attatchedImage, units[i].xpos, units[i].ypos);
        }
        /**
         * <summary>Utility method to draw the lines between all actioned units and their action target</summary>
         */
        private void DrawLines(Graphics graphics,Color drawColor)
        {

            int i = 0;
            while (i < actingUnit)
            {
                graphics.DrawLine(new Pen(drawColor), lineDraw[i * 4], lineDraw[(i * 4) + 1], lineDraw[(i * 4) + 2], lineDraw[(i * 4) + 3]);
                i++;
            }
        }
        /**
         * <summary>When using the 'paint on mouse' mosue down enviroment, tells the program to stop drawing</summary>
         */
        private void PainterForm_MouseUp(object sender, MouseEventArgs e)
        {
            shouldPaint = false;
        }
        /**
         * <summary>Animates the potential target of selected unit if a unit is selected. Draws continously if mouse down and 'paint on mouse' trues</summary>
         */
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
                    /* 
                    *DRAW TERRAIN 
                    * 
                    */
                    DrawTerrain(graphics, ammount);
                    /*
                     * END OF DRAW TERRAIN
                     * 
                     */
                    int i = 0;
                    while (i < currentUnits)
                    {
                        DrawUnits(graphics,i);
                        i++;

                    }
                    DrawLines(graphics, drawColor);
                    graphics.DrawLine(new Pen(drawColor), units[selectedUnit].xpos + 25, units[selectedUnit].ypos + 25, e.X, e.Y);
                    

                }
            }
            graphics.Dispose();
        }
        /**
        * <summary>actionEnemy(int unit):
        * Randomizes an action, then has the passed unit (an index in the 'units' array) perform that action if not dead, updating details similar to <see cref="button2_Click"/></see> 
        * <param name="unit">The index of the unit object in the 'units' array that should be passed. This is the unit that will be actioned</param>
        * 
        * </summary>
        */
        //Kill Move - 'K' | Shooter index | shot unit index
        //Move Commnad - 'M| Moving unit's index | 4 digits, leading zeros, x move co-ordinate | 4 digits, leading zeros, Y move co-ordinate
        private String actionEnemy(int unit)
        {
            if (units[unit].dead == true)
            {
                return "E" + unit;
            }
            else
            {
                int actionTaken = rand.Next(2) +1;
                Console.Write("\n action taken is generated as "+actionTaken);
                switch (actionTaken) {
                    //Shoot at nearest team 1 unit
                    case 1:
                        int i = 0, bestDif = windowHeight+windowLength, bestUnit = -1;
                        while (i < currentUnits)
                        {
                            int diffrence = 0, target = -1;
                            if (units[i].team == 1)
                            {

                                diffrence = units[unit].xpos - units[i].ypos;
                                if (diffrence < 0)
                                {
                                    actionTaken = actionTaken * -1;
                                }
                                actionTaken = units[unit].ypos - units[i].ypos;
                                if (actionTaken < 0)
                                {
                                    actionTaken =actionTaken *  -1;
                                }
                                diffrence = diffrence + actionTaken;
                                Console.Write("dif end result" + diffrence +"comapred to "+bestDif+"\n");
                                if (bestDif > diffrence)
                                {
                                    bestUnit = i;
                                    bestDif = diffrence;
                                }
                                actionTaken = 1;
                            }
                            i++;
                        }
                        if (bestUnit != -1) {
                            return "K" + unit + bestUnit+"\n";
                        }
                        else
                        {

                            return "E"+unit;
                        }
                        break;

                    case 2:
                        return "M"+unit+rand.Next(windowLength-50).ToString("D4")+( rand.Next(windowHeight - 50)+toolBar.Height).ToString("D4") + "\n";
                        break;

                    default:
                        return "E" + unit; ;
                        Console.Write("\nError hit with console\n");
                        break;


                }
            }

        }
            /**
             * <summary>Utility method to handle any clicks on the toolbar. Checks aganst the button's 'Name' value</summary>
             */

            private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            String eName = e.Button.Text;
            String name = "Default";
            int team = 1;
            Console.Write("\n"+eName+" \n ------\n");
            if (e.Button == toolButton1)
            {
                    name = "Hero1";
                    team = 1;
            }
            else if (e.Button == toolButton2 )
            {
                name = "BadMan1";
                team = 2;
            }
            else if(currentUnits > 0)
            {

                name = "Default";
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    Bitmap image = null;
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|";//ALternative - |All files (*.*)|*.*
                    openFileDialog.FilterIndex = 10;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        String filePath = openFileDialog.FileName;
                        Console.Write("\n filepath is "+filePath);
                        image = new Bitmap(filePath);
                        if(image.Height>50 || image.Width > 50)
                        {
                            textBox1.Text = "File can only be up to 50x50 in size";
                        }
                        else
                        {

                            units[currentUnits - 1].attatchedImage = image;
                            Graphics graphics = CreateGraphics();
                            graphics.Clear(Color.White);
                            Console.Write("\n Drawing unit");
                            DrawTerrain(graphics,ammount);
                            DrawUnits(graphics, currentUnits - 1);
                            graphics.Dispose();
                        }
                    }
                }
            }
            if ((currentUnits < units.Length && ! name.Equals("Default")))
            {
                Console.Write("\n Building unit with image location " + "Pictures/" + name + ".png");
                units[currentUnits] = new Unit(new Bitmap("Pictures/" + name + ".png"), rand.Next(windowLength - 50), rand.Next(windowHeight - 50) + this.toolBar.Height,team, rand.Next(3) + 1);
                Graphics graphics = CreateGraphics();
                Console.Write("\n Drawing unit");
                DrawUnits(graphics, currentUnits);
                graphics.Dispose();
                currentUnits++;
            }

        }
        /**
         * <summary> A utility function to paly a sound</summary> <param name="type"> The type of sound. 1=move 2=shoot</param>
         * 
         */
        private void playSound(int type)
        {
            String name = "";
            if (type == 1)
            {
                name = "move";
            }else if (type == 2)
            {
                name = "shoot";
            }
            if (type != 0)
            {
                name = "Audio/" + name + ".wav";
                Console.Write("\n  Playing " + name +"\n");
                try
                {
                    SoundPlayer simpleSound = new SoundPlayer(@name);
                    simpleSound.Play();
                }
                catch (Exception e)
                {
                    Console.Write("File error when palying music : " + e.Message + "\n");
                }
            }
            
            
        }
        /**
         * <summary> Listner class tat handles and stored 'Command strings', and they changes they produce, drawing any required.<para/> CommandStrings are built in <see cref="PainterForm_MouseDown">Mouse down</see></summary>
         */
        private async void button2_Click(object sender,EventArgs e)
        {


            Graphics graphics = CreateGraphics();
            graphics.Clear(Color.White);
            DrawTerrain(graphics,ammount);
            Console.Write("\n acting unit is " + actingUnit);
            int i = 0;
            i = 0;
            while (i < actingUnit)
            {
                Console.Write("\n Pre-muddle, Unit " + moveCommands[i].Substring(1, 1) + "with initiative "+units[Int32.Parse(moveCommands[i].Substring(1, 1))].init + " goes next ");
                i++;
            }
            i = 0;
            while (i < currentUnits)
            {
                if (units[i].team == 2)
                {
                    moveCommands[actingUnit] = actionEnemy(i);
                    Console.Write("\n A.I command added and it is" + moveCommands[actingUnit] + " | acting unit is " + i);
                    actingUnit++;
                }
                i++;
            }
            moveCommands = sortMoves(moveCommands);
            i = 0;
            while (i < actingUnit)
            {
                Console.Write("\n Post-muddle, Unit " + moveCommands[i].Substring(1, 1) + "with initiative " + units[Int32.Parse(moveCommands[i].Substring(1, 1))].init + " goes next ");
                i++;
            }
            i = actingUnit-1;
            while (i >= 0){
                int currentActingUnit = Int32.Parse(moveCommands[i].Substring(1, 1));
                Console.Write("MoveOrder[" + i + "] is \t" + moveCommands[i]);
                if (! units[currentActingUnit].dead)
                {
                    if (moveCommands[i].Substring(0,1).Equals("E"))
                    {
                        Console.Write(" \n Exit Command \n");
                    }
                    /*
                     * MOVE COMMAND
                     */
                    else if (moveCommands[i].Substring(0, 1).Equals("M"))
                    {
                        audioQueue[Int32.Parse(moveCommands[i].Substring(1, 1))] = 1;
                        Console.Write(" Moving unit " + currentActingUnit + " to " + moveCommands[i].Substring(2, 4) + " " + moveCommands[i].Substring(6, 4) + "\n");
                        Console.Write(" Moving unit " + currentActingUnit + " to " + (Int32.Parse(moveCommands[i].Substring(2, 4)) - 25) + " " + (Int32.Parse(moveCommands[i].Substring(6, 4)) - 25));
                        units[currentActingUnit].xpos = Int32.Parse(moveCommands[i].Substring(2, 4)) - 25;
                        units[currentActingUnit].ypos =( Int32.Parse(moveCommands[i].Substring(6, 4)) - 25)+this.toolBar.Height;
                        Console.Write("Unit deets are " + units[currentActingUnit].xpos + "," + units[currentActingUnit].ypos);
                    }
                    /*
                     * SHOOT COMMAND
                     */
                    else if (moveCommands[i].Substring(0, 1).Equals("K"))
                    {
                        audioQueue[Int32.Parse(moveCommands[i].Substring(1, 1))] = 2;
                        int shotChance = units[currentActingUnit].acc + rand.Next(100);
                        Console.Write("\n shotchance pre-terrain "+shotChance);
                        int targetTile = 0;
                        targetTile = units[Int32.Parse(moveCommands[i].Substring(2, 1))].xpos / (windowLength / ammount);
                        targetTile = (ammount * targetTile) + units[Int32.Parse(moveCommands[i].Substring(2, 1))].ypos / (windowHeight / ammount);
                        switch (gridTypeDownRight[targetTile]) {

                            case 1:
                                shotChance -= 10;
                                break;
                            case 2:
                                shotChance -= 20;
                                break;

                            default:
                                break;

                        }
                        Console.Write("\n shotchance post-terrain " + shotChance);

                        if (shotChance>100)
                        {

                            units[Int32.Parse(moveCommands[i].Substring(2,1))].attatchedImage = new Bitmap("Pictures/Corpse" + ((rand.Next(100) % 3) + 1) + ".png");
                            units[Int32.Parse(moveCommands[i].Substring(2, 1))].dead = true;
                        }
                        else
                        {
                            Console.Write("\n Missed the shot \n");
                        }
                    }
                    /*
                     * DEFAULT COMMAND
                     */
                    else
                    {
                        Console.Write("Command error");
                    }
                }
                i--;
            }
            i = 0;
            while (i < currentUnits)
            {
                Console.Write("End of turn unit " + i + " acting");
                units[i].acted = false;
                Console.Write("change occured unit " + i + " acting is set to " + units[i].acted + " \n");
                await Task.Delay(500);
                if (audioQueue[i] != 0)
                {
                    playSound(audioQueue[i]);
                }
                DrawUnits(graphics, i);
                i++;

            }
            
            //CLEAN UP
            graphics.Dispose();
            audioQueue = new int[maxUnits];
            moveCommands = new String[moveCommands.Length];
            moveOrder = new int[moveOrder.Length];
            lineDraw = new int[lineDraw.Length];
            actingUnit = 0;
        }
        #endregion

    }
}

