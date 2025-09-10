namespace Creatures
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Creature creature = new Villager();
            Creature monster = new Monster();
            Creature player = new Player();

            monster.Attack(player);
            monster.Attack(player);
            monster.Attack(player);
            monster.Attack(player);

            //Creature monsterAttack = new Monster();
            //monsterAttack.Attack(player);
            ////MessageBox.Show(player.Say());
            //monsterAttack.Attack(player);
            ////MessageBox.Show(player.Say());
            //monsterAttack.Attack(player);
            ////MessageBox.Show(player.Say());
            //monsterAttack.Attack(player);
            //MessageBox.Show(player.Say());

            //MessageBox.Show(monster.Say());
        }
    }
}
