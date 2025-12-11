using System;

[Serializable]
public class DonutInGameData
{
    public string uid;          //고유 ID
    public string origin;       //도넛의 기본 데이터 ID
    public int level;           //현재 도넛의 레벨
    public int atk;             //현재 도넛의 공격력
    public int def;             //현재 도넛의 방어력
    public int hp;              //현재 도넛의 체력
    
    public float crit;          //현재 도넛의 크리티컬 확률

    public float slingShotPower;

    public DonutInGameData() { }
    public DonutInGameData(DonutInstanceData instanceData)
    {
        uid = instanceData.uid;
        origin = instanceData.origin;
        level = instanceData.level;
        atk = instanceData.atk;
        def = instanceData.def;
        hp = instanceData.hp;
        
        crit = instanceData.crit;

        slingShotPower = 1f;
    }
}
