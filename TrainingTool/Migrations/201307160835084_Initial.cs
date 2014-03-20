namespace TrainingTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        testId = c.String(),
                        content = c.String(),
                        chooseA = c.String(),
                        chooseB = c.String(),
                        chooseC = c.String(),
                        chooseD = c.String(),
                        answer = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.testLists",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        courseID = c.Int(nullable: false),
                        test1ID = c.String(),
                        test2ID = c.String(),
                        test3ID = c.String(),
                        test4ID = c.String(),
                        test5ID = c.String(),
                        test6ID = c.String(),
                        test7ID = c.String(),
                        test8ID = c.String(),
                        test9ID = c.String(),
                        test10ID = c.String(),
                        editTime = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.saveRecords", "readChapter", c => c.Int(nullable: false));
            AddColumn("dbo.saveRecords", "learnMinutes", c => c.Double(nullable: false));
            DropColumn("dbo.saveRecords", "toContent");
        }
        
        public override void Down()
        {
            AddColumn("dbo.saveRecords", "toContent", c => c.String());
            DropColumn("dbo.saveRecords", "learnMinutes");
            DropColumn("dbo.saveRecords", "readChapter");
            DropTable("dbo.testLists");
            DropTable("dbo.Tests");
        }
    }
}
