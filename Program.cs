// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Main();

/// <summary>
/// 邏輯說明: 以 ProdNo = "A_Prod" 為例，總庫存數量為 10，分別放到4個儲位，第一個AA放5個，BB放3個，CC放2個，DD放1個，總共10個，和總庫存數量相同。
///	輸出結果: 最後要取得各個儲位和庫存的占比。
///	規則: 由小排序到大，非最大的數量使用四捨五入取得占比，而是最大的數量使用無條件捨去取得占比，最後一個就用100去扣除之前的占比。	
/// </summary>
void Main()
{
	var oriDetailData = new List<StoreModel> { 
        //  A_Prod
		new StoreModel { Id = 1, ProdNo = "A_Prod", StoreName = "AA", StockQty = 10, StoreQty = 4 },    //  40%
		new StoreModel { Id = 2, ProdNo = "A_Prod", StoreName = "BB", StockQty = 10, StoreQty = 3 },    //  30%
		new StoreModel { Id = 3, ProdNo = "A_Prod", StoreName = "CC", StockQty = 10, StoreQty = 2 },    //  20%
		new StoreModel { Id = 4, ProdNo = "A_Prod", StoreName = "DD", StockQty = 10, StoreQty = 1 },    //  10%
                                                                                                              
        //  B_Prod                                                                                            
		new StoreModel { Id = 5, ProdNo = "B_Prod", StoreName = "AA", StockQty = 22, StoreQty = 7 },    //  31%
		new StoreModel { Id = 6, ProdNo = "B_Prod", StoreName = "BB", StockQty = 22, StoreQty = 7 },    //  31%
		new StoreModel { Id = 7, ProdNo = "B_Prod", StoreName = "CC", StockQty = 22, StoreQty = 3 },    //  14%
		new StoreModel { Id = 8, ProdNo = "B_Prod", StoreName = "DD", StockQty = 22, StoreQty = 3 },    //  14%
		new StoreModel { Id = 9, ProdNo = "B_Prod", StoreName = "EE", StockQty = 22, StoreQty = 1 },    //  5%
		new StoreModel { Id = 10, ProdNo = "B_Prod", StoreName = "FF", StockQty = 22, StoreQty = 1 },   //  5%
                                                                                                              
        //  C_Prod                                                                                            
        new StoreModel { Id = 11, ProdNo = "C_Prod", StoreName = "AA", StockQty = 22, StoreQty = 2 },   //  9%
        new StoreModel { Id = 12, ProdNo = "C_Prod", StoreName = "BB", StockQty = 22, StoreQty = 2 },   //  9%
        new StoreModel { Id = 13, ProdNo = "C_Prod", StoreName = "CC", StockQty = 22, StoreQty = 2 },   //  9%
        new StoreModel { Id = 14, ProdNo = "C_Prod", StoreName = "DD", StockQty = 22, StoreQty = 16 },  //  73%
                                                                                                              
        //  D_Prod                                                                                            
        new StoreModel { Id = 15, ProdNo = "D_Prod", StoreName = "AA", StockQty = 22, StoreQty = 2 },   //  9%
        new StoreModel { Id = 16, ProdNo = "D_Prod", StoreName = "BB", StockQty = 22, StoreQty = 2 },   //  9%
        new StoreModel { Id = 17, ProdNo = "D_Prod", StoreName = "CC", StockQty = 22, StoreQty = 2 },   //  9%
        new StoreModel { Id = 18, ProdNo = "D_Prod", StoreName = "DD", StockQty = 22, StoreQty = 8 },   //  36%
        new StoreModel { Id = 19, ProdNo = "D_Prod", StoreName = "EE", StockQty = 22, StoreQty = 8 },   //  37%
    };

    var result = oriDetailData
                .GroupBy(x => new { x.ProdNo })   
                .SelectMany((x) =>
                        x.Select((q, index) =>
                        {
                            //  於群組中，原始集合紀錄的當前總占比
                            var groupPersentSum = x.Sum(p => p.StorePersent);

                            //  於群組中，取得群組的總筆數
                            var groupCount = x.Count();

                            //  於群組中，取得最大的數量
                            var groupMaxQty = x.Max(p => p.StoreQty);

                            //  於群組中，依照數量小到大排序取得物件
                            var currentItem = x.OrderBy(z => z.StoreQty).Skip(index).FirstOrDefault();

                            //  於群組中，依照數量小到大排序取得該物件的占比，使用"四捨五入法"
                            var currentStoreRate = (Math.Round(currentItem.StoreQty / x.FirstOrDefault().StockQty, 2) * 100);

                            //  若是最後一筆(序列 + 1 == 總筆數)
                            if (index + 1 == groupCount)
                            {
                                currentStoreRate = 100 - Convert.ToDecimal(groupPersentSum);
                            }
                            //  若當前的數量 == 最大數量，就使用"無條件捨去"
                            else if (currentItem.StoreQty == groupMaxQty)
                            {
                                currentStoreRate = Math.Floor((currentItem.StoreQty / x.FirstOrDefault().StockQty) * 100);
                            }

                            //  於群組中，使用原始集合紀錄占比
                            currentItem.StorePersent = currentStoreRate;

                            return new StoreModel
                            {
                                Id = currentItem.Id,
                                ProdNo = x.Key.ProdNo,
                                StockQty = currentItem.StockQty,
                                StoreQty = currentItem.StoreQty,
                                StoreName = currentItem.StoreName,
                                StorePersent = currentStoreRate
                            };
                        })
            ).ToList();

    Console.WriteLine("Done");
}


public class StoreModel
{
	//	[PK] 序號
	public int Id { get; set; }

	//	[PK] 商品編號
	public string? ProdNo { get; set; }

	//	儲位名稱
	public string? StoreName { get; set; }

	//	庫存總數量
	public decimal StockQty { get; set; }

	//	各儲位數量
	public decimal StoreQty { get; set; }

    //	各儲位占比
    public decimal StorePersent { get; set; }
}