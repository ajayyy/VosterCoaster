using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper {

    //modified from https://answers.unity.com/questions/1299082/transformeulerangles-x-issues.html
    //This function converts a quaternion to a euler angle without triggering gimbal lock
    public static Vector3 ConvertQuant2Euler(Quaternion quaternion) {
        float tempEuler;
        float[] eulerAngles = new float[3];

        //Convert pitch - X
        tempEuler = Mathf.Atan2(2 * quaternion.x * quaternion.w + 2 * quaternion.y * quaternion.z, 1 - 2 * quaternion.x * quaternion.x - 2 * quaternion.z * quaternion.z);
        eulerAngles[0] = tempEuler * 180 / Mathf.PI;

        //Convert yaw - Y
        tempEuler = Mathf.Asin(2 * quaternion.x * quaternion.y + 2 * quaternion.z * quaternion.w);
        eulerAngles[1] = tempEuler * 180 / Mathf.PI;

        //Convert roll - Z
        tempEuler = Mathf.Atan2(2 * quaternion.y * quaternion.w + 2 * quaternion.x * quaternion.z, 1 - 2 * quaternion.y * quaternion.y - 2 * quaternion.z * quaternion.z);
        eulerAngles[2] = tempEuler * 180 / Mathf.PI;

        return new Vector3(eulerAngles[0], eulerAngles[1], eulerAngles[2]);
    }
}
