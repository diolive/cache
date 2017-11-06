namespace DioLive.BlackMint.Persistence.SQLite
{
    internal static class Queries
    {
        private const string SelectIdentity = "SELECT last_insert_rowid();";

        public static class User
        {
            public const string TableName = "`Users`";
            public const string Add = "INSERT INTO " + TableName + " (`DisplayName`) VALUES (@displayName);" + SelectIdentity;
            public const string Get = "SELECT * FROM " + TableName + " WHERE `Id`=@id LIMIT 1;";
        }

        public static class UserIdentity
        {
            public const string TableName = "`UserIdentities`";
            public const string Add = "INSERT INTO " + TableName + " (`NameIdentity`, `UserId`) VALUES (@nameIdentity, @userId);" + SelectIdentity;
            public const string Get = "SELECT * FROM " + TableName + " WHERE `NameIdentity`=@nameIdentity LIMIT 1;";
        }

        public static class Book
        {
            public const string TableName = "`Books`";
            public const string Add = "INSERT INTO " + TableName + " (`Name`, `AuthorId`) VALUES (@name, @authorId);" + SelectIdentity;
            public const string Get = "SELECT * FROM " + TableName + " WHERE `Id`=@id LIMIT 1;";
            public const string GetAccessible = "SELECT b.* FROM " + TableName + " b INNER JOIN " + BookAccess.TableName + " ba ON b.`Id`=ba.`BookId` WHERE ba.`UserId`=@userId;";
            public const string UpdateName = "UPDATE " + TableName + " SET `Name`=@name WHERE `Id`=@id;";
            public const string Delete = "DELETE FROM " + TableName + " WHERE `Id`=@id;";
        }

        public static class BookAccess
        {
            public const string TableName = "`BookAccess`";
            public const string Add = "INSERT INTO " + TableName + " (`BookId`, `UserId`, `Role`) VALUES (@bookId, @userId, @role);" + SelectIdentity;
            public const string GetRole = "SELECT `Role` FROM " + TableName + " WHERE `BookId`=@bookId AND `UserId`=@userId LIMIT 1;";
            public const string Update = "UPDATE " + TableName + " SET `Role`=@role WHERE `BookId`=@bookId AND `UserId`=@userId;";
            public const string Delete = "DELETE FROM " + TableName + " WHERE `BookId`=@bookId AND `UserId`=@userId;";
        }

        public static class Income
        {
            public const string TableName = "`Incomes`";
            public const string Add = "INSERT INTO " + TableName + " (`BookId`, `Source`, `Date`, `Value`, `Currency`, `Comments`) VALUES (@bookId, @source, @date, @value, @currency, @comments);" + SelectIdentity;
            public const string GetOrdered = "SELECT * FROM " + TableName + " WHERE `BookId`=@bookId ORDER BY {0} LIMIT @Limit OFFSET @Offset;";
        }

        public static class Purchase
        {
            public const string TableName = "`Purchases`";
            public const string Add = "INSERT INTO " + TableName + " (`BookId`, `Seller`, `Date`, `TotalCost`, `Currency`, `Comments`) VALUES (@BookId, @Seller, @Date, 0, @Currency, @Comments);" + SelectIdentity;
            public const string Get = "SELECT * FROM " + TableName + " WHERE `Id`=@id LIMIT 1;";
            public const string GetOrdered = "SELECT * FROM " + TableName + " WHERE `BookId`=@bookId ORDER BY {0} LIMIT @Limit OFFSET @Offset;";
            public const string GetRole = "SELECT ba.`Role` FROM " + BookAccess.TableName + " ba INNER JOIN " + TableName + " p ON p.`BookId`=ba.`BookId` WHERE p.`Id`=@purchaseId AND ba.`UserId`=@userId LIMIT 1;";
            public const string Update = "UPDATE " + TableName + " SET `Seller`=@Seller, `Date`=@Date, `Currency`=@Currency, `Comments`=@Comments WHERE `Id`=@Id;";
        }

        public static class PurchaseItem
        {
            public const string TableName = "`PurchaseItems`";
            public const string Add = "INSERT INTO " + TableName + " (`PurchaseId`, `Name`, `Price`, `Count`) VALUES (@PurchaseId, @Name, @Price, @Count);" + SelectIdentity;
            public const string Get = "SELECT * FROM " + TableName + " WHERE `Id`=@id LIMIT 1;";
            public const string GetByPurchase = "SELECT * FROM " + TableName + " WHERE `PurchaseId`=@purchaseId;";
            public const string GetRole = "SELECT ba.`Role` FROM " + BookAccess.TableName + " ba INNER JOIN " + Purchase.TableName + " p ON p.`BookId`=ba.`BookId` INNER JOIN " + TableName + " pi ON pi.`PurchaseId`=p.`Id` WHERE pi.`Id`=@purchaseItemId AND ba.`UserId`=@userId LIMIT 1;";
        }

        public static class Currency
        {
            public const string TableName = "`Currencies`";
            public const string Get = "SELECT * FROM " + TableName + " WHERE `Code`=@code LIMIT 1;";
            public const string GetAll = "SELECT * FROM " + TableName + ";";
        }
    }
}