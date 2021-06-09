using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 필드에 배치할 수 있는 오브젝트의 최상위 클래스
/// </summary>
public class EditingObject : CustomMonoBehaviour
{
    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator == (EditingObject v1, EditingObject v2)
    {
        return v1?.position3 == v2?.position3;
    }

    public static bool operator !=(EditingObject v1, EditingObject v2)
    {
        return !(v1 == v2);
    }
}
