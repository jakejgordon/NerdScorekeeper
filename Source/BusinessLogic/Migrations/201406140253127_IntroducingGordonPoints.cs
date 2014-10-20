namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class IntroducingGordonPoints : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerGameResult", "GordonPoints", c => c.Int(nullable: false));
            Sql("UPDATE PlayerGameResult SET GordonPoints = (PlayedGame.NumberOfPlayers + 1) - PlayerGameResult.GameRank "
                + "FROM PlayerGameResult INNER JOIN PlayedGame ON PlayerGameResult.PlayedGameId = PlayedGame.Id");
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerGameResult", "GordonPoints");
        }
    }
}
