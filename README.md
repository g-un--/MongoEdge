MongoEdge
=========
#### .Net wrapper for nodejs MongoDB driver using EdgeJs

    class Program
    {
        static void Main(string[] args)
        {
            var connectToMongo = Edge.Func(@"
            
                var MongoClient = require('mongodb').MongoClient;
                
                return function(databaseName, task) {
                    MongoClient.connect('mongodb://localhost/' + databaseName, function(err, db) {
                        if(err) task(null, err);

                        return task(null, {
                            version: function(data, versionTask) { 
                                         db.admin().buildInfo(function(infoErr, info) {
                                            versionTask(infoErr, info.version);
                                         });
                                     },
                            insert: function(data, insertTask) {
                                        db.collection(data.collection).insert(data.values, function(insertErr, result) {
                                            insertTask(insertErr, result);
                                        });
                                    },
                            drop: function(data, dropTask) {
                                        db.dropCollection(data.collection, function(dropErr, result) {
                                            dropTask(dropErr, result);
                                        });
                                  },
                            close: function(data, closeTask) { db.close(); closeTask(); }
                        });
                    });
                }");

            
            var db = connectToMongo("edge").Result;
            
            Task.Run(
                Test.WithStopwatch(
                    () => TestGetVersion(db),
                    "get version")).Wait();

            Task.Run(
                Test.WithStopwatch(
                    () => TestInsert(
                            db,
                            "edge",
                            Enumerable.Range(1, 1000)
                                .Select(i => new { Index = i, UUID = Guid.NewGuid() })
                                .ToArray()),
                    "insert 1000")).Wait();

            Task.Run(
                Test.WithStopwatch(
                    () => TestDrop(db, "edge"), "drop db")).Wait();

            Task.Run(
                Test.WithStopwatch(
                    () => TestClose(db),
                    "close db")).Wait();

            Console.ReadLine();
        }

        public static async Task TestGetVersion(dynamic db)
        {
            var dbVersion = await db.version(new { });
        }

        public static async Task TestInsert(dynamic db, string collection, object[] values)
        {
            await db.insert(new { collection = collection, values = values });
        }

        public static async Task TestDrop(dynamic db, string collection)
        {
            await db.drop(new { collection = collection });
        }

        public static async Task TestClose(dynamic db)
        {
            await db.close(new { });
        }
    }
