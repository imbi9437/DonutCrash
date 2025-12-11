using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MerchandiseType
{
    ExchangeGold,   //다이아 => 골드 교환
    ExchangeCash,   //현금 => 다이아 교환
    BuyGold,        //골드 => 아이템 구매
    BuyCash,        //다이아 => 아이템 구매
    Package,        //현금, 다이아, 골드 => 패키지 구매
}

public enum PriceType
{
    Gold,
    Cash,
    RecipePieces,
    Money
}

/// <summary>상점에서 판매하는 상품 데이터</summary>

[Serializable]
public class MerchandiseData
{
    public string uid;                      //고유 UID
    public string merchandiseName;          //상품 이름
    public MerchandiseType merchandiseType; //상품 타입
    public PriceType priceType;             //가격 타입
    public int price;                       //가격
    public int stockCount;                  //구매 가능 개수
    public List<string> productIds = new();         //판매 제품 ID
    public string resourcePath;             //관련 에셋 위치
}

[FirestoreData]
public class MerchandiseDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string merchandiseName { get; set; }
    [FirestoreProperty] public MerchandiseType merchandiseType { get; set; }
    [FirestoreProperty] public PriceType priceType { get; set; }
    [FirestoreProperty] public int price { get; set; }
    [FirestoreProperty] public int stockCount { get; set; }
    [FirestoreProperty] public List<string> productIds { get; set; }
    [FirestoreProperty] public string resourcePath { get; set; }

    public static MerchandiseDataDto CurrentDto(MerchandiseData data)
    {
        return new MerchandiseDataDto()
        {
            uid = data.uid,
            merchandiseName = data.merchandiseName,
            merchandiseType = data.merchandiseType,
            priceType = data.priceType,
            price = data.price,
            stockCount = data.stockCount,
            productIds = data.productIds,
            resourcePath = data.resourcePath
        };
    }
}