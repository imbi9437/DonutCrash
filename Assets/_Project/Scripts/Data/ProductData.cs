using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public enum ProductType
{
    Gold,
    Cash,
    Ingredient,
    Donut,
    Baker,
    TrayExpand,
    Recipe,
    RecipePiece,
}


[Serializable]
public class ProductData
{
    public string uid;                  //제품의 고유 UID
    public ProductType productType;     //제품의 타입
    public string productId;            //제품의 제공 아이템 UID (UID가 필요한 경우만)
    public int productValue;            //제품 개수
    public string resourcePath;         //관련 에셋 위치
}

[FirestoreData]
public class ProductDataDto
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public ProductType productType { get; set; }
    [FirestoreProperty] public string productId { get; set; }
    [FirestoreProperty] public int productValue { get; set; }
    [FirestoreProperty] public string resourcePath { get; set; }

    public static ProductDataDto CurrentDto(ProductData data)
    {
        return new ProductDataDto()
        {
            uid = data.uid,
            productType = data.productType,
            productId = data.productId,
            productValue = data.productValue,
            resourcePath = data.resourcePath
        };
    }
}