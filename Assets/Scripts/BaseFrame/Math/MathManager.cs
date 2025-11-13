
using System;
using UnityEngine;
using UnityEngine.Events;

public class MathManager : SingletonManager<MathManager>
{
    private MathManager() { }
    /// <summary>
    /// 角度转弧度
    /// </summary>
    /// <param name="deg">角度</param>
    /// <returns></returns>
    public static float Deg2Rad(float deg) { return deg * Mathf.Deg2Rad; }
    /// <summary>
    /// 弧度转角度
    /// </summary>
    /// <param name="rad">弧度</param>
    /// <returns></returns>
    public static float Rad2Deg(float rad) { return rad * Mathf.Rad2Deg; }
    /// <summary>
    /// 计算2D世界物体之间的距离
    /// </summary>
    /// <param name="a">物体A</param>
    /// <param name="b">物体B</param>
    /// <returns></returns>
    public static float GetDistance2D(Vector3 a, Vector3 b)
    {
        a.z = 0;
        b.z = 0;
        return Vector3.Distance(a, b);
    }
    /// <summary>
    /// 判断2D世界物体之间的距离是否小于一段距离
    /// true说明 ab小
    /// false说明 ab大
    /// </summary>
    /// <param name="a">物体A</param>
    /// <param name="b">物体B</param>
    /// <param name="distance">距离值</param>
    /// <returns></returns>
    public static bool JudgeDistance2D(Vector3 a, Vector3 b, float distance)
    {
        return GetDistance2D(a, b) <= distance;
    }
    /// <summary>
    /// 计算3D世界物体之间的距离
    /// </summary>
    /// <param name="a">物体A</param>
    /// <param name="b">物体B</param>
    /// <returns></returns>
    public static float GetDistance3D(Vector3 a, Vector3 b)
    {
        a.y = 0;
        b.y = 0;
        return Vector3.Distance(a, b);
    }
    /// <summary>
    /// 判断3D世界物体之间的距离是否小于一段距离
    /// true说明 ab小
    /// false说明 ab大
    /// </summary>
    /// <param name="a">物体A</param>
    /// <param name="b">物体B</param>
    /// <param name="distance">距离值</param>
    /// <returns></returns>
    public static bool JudgeDistance3D(Vector3 a, Vector3 b, float distance)
    {
        return GetDistance3D(a, b) <= distance;
    }
    /// <summary>
    /// 判断某一点是否在摄像机视角外
    /// </summary>
    /// <param name="camera">指定摄像机</param>
    /// <param name="pos">计算点</param>
    /// <returns></returns>
    public static bool IsWorldPosOutScreen(Camera camera, Vector3 pos)
    {
        Vector3 screenPos = camera.WorldToScreenPoint(pos);
        //如果在屏幕内
        if (screenPos.x >= 0 && screenPos.x < Screen.width &&
            screenPos.y >= 0 && screenPos.y < Screen.height) return false;
        return true;
    }
    /// <summary>
    /// 判断目标点是否在以玩家点为中心的扇形范围内
    /// </summary>
    /// <param name="playerPos">玩家点</param>
    /// <param name="playerForward">玩家正朝向</param>
    /// <param name="targetPos">目标点</param>
    /// <param name="radius">扇形半径</param>
    /// <param name="angle">扇形角度</param>
    /// <returns></returns>
    public static bool IsInSectorRange(Vector3 playerPos,Vector3 playerForward,Vector3 targetPos,float radius,float angle)
    {
        playerPos.y = 0;
        playerForward.y = 0;
        targetPos.y = 0;
        return Vector3.Distance(playerPos, targetPos) <= radius && Vector3.Angle(playerForward, targetPos - playerPos) <= angle / 2;
    }
    /// <summary>
    /// 射线检测
    /// 获取 RaycastHit 对象信息
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级遮罩</param>
    public static void RayCast(Ray ray, UnityAction<RaycastHit> callback,float maxDistance,int layerMask)
    {
        if (Physics.Raycast(ray, out var hit, maxDistance, layerMask)) callback?.Invoke(hit);
    }
    /// <summary>
    /// 射线检测
    /// 获取 GameObject 对象信息
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级遮罩</param>
    public static void RayCast(Ray ray, UnityAction<GameObject> callback,float maxDistance,int layerMask)
    {
        if (Physics.Raycast(ray, out var hit, maxDistance, layerMask)) callback?.Invoke(hit.collider.gameObject);
    }
    /// <summary>
    /// 泛型射线检测
    /// 获取 T 对象信息
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级遮罩</param>
    public static void RayCast<T>(Ray ray, UnityAction<T> callback,float maxDistance,int layerMask)
    {
        if (Physics.Raycast(ray, out var hit, maxDistance, layerMask)) callback?.Invoke(hit.collider.gameObject.GetComponent<T>());
    }
    /// <summary>
    /// 穿透射线检测
    /// 获取所有 RaycastHit 对象信息
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级遮罩</param>
    public static void RayCastAll(Ray ray,UnityAction<RaycastHit> callback,float maxDistance,int layerMask)
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(ray,maxDistance,layerMask);
        for (int i = 0; i < raycastHits.Length; i++) callback?.Invoke(raycastHits[i]);
    }
    /// <summary>
    /// 穿透射线检测
    /// 获取所有 GameObject 对象信息
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级遮罩</param>
    public static void RayCastAll(Ray ray,UnityAction<GameObject> callback,float maxDistance,int layerMask)
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(ray,maxDistance,layerMask);
        for (int i = 0; i < raycastHits.Length; i++) callback?.Invoke(raycastHits[i].collider.gameObject);
    }
    /// <summary>
    /// 泛型穿透射线检测
    /// 获取所有 T 对象信息
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callback">回调</param>
    /// <param name="maxDistance">最大距离</param>
    /// <param name="layerMask">层级遮罩</param>
    public static void RayCastAll<T>(Ray ray,UnityAction<T> callback,float maxDistance,int layerMask)
    {
        RaycastHit[] raycastHits = Physics.RaycastAll(ray,maxDistance,layerMask);
        for (int i = 0; i < raycastHits.Length; i++) callback?.Invoke(raycastHits[i].collider.gameObject.GetComponent<T>());
    }
    /// <summary>
    /// 盒状范围检测
    /// </summary>
    /// <param name="center">中心点</param>
    /// <param name="half">长宽高的一半</param>
    /// <param name="quaternion">角度</param>
    /// <param name="layerMask">遮罩</param>
    /// <param name="callback">回调</param>
    /// <typeparam name="T"></typeparam>
    public static void OverlapBox<T>(Vector3 center,Vector3 half,Quaternion quaternion,int layerMask,UnityAction<T> callback) where T : class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapBox(center,half,quaternion,layerMask,QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (type == typeof(Collider)) callback?.Invoke(colliders[i] as T);
            else if (type == typeof(GameObject)) callback?.Invoke(colliders[i].gameObject as T);
            else callback?.Invoke(colliders[i].gameObject.GetComponent<T>());
        }
    }
    /// <summary>
    /// 球状范围检测
    /// </summary>
    /// <param name="center">中心点</param>
    /// <param name="half">半径</param>
    /// <param name="layerMask">遮罩</param>
    /// <param name="callback">回调</param>
    /// <typeparam name="T"></typeparam>
    public static void OverlapSphere<T>(Vector3 center,float radius,int layerMask,UnityAction<T> callback) where T : class
    {
        Type type = typeof(T);
        Collider[] colliders = Physics.OverlapSphere(center,radius,layerMask,QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (type == typeof(Collider)) callback?.Invoke(colliders[i] as T);
            else if (type == typeof(GameObject)) callback?.Invoke(colliders[i].gameObject as T);
            else callback?.Invoke(colliders[i].gameObject.GetComponent<T>());
        }
    }
}
