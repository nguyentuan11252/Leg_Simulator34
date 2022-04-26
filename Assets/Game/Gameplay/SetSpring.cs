using UnityEngine;

public class SetSpring : MonoBehaviour
{
    private HingeJoint Joint;
    public Transform Target;

    [Space(20)]
    public float Compensador;
    public bool Inverter;
    public enum Eixo { X,Y,Z};
    public Eixo Sentido = Eixo.Z;

    void Start()
    {
        Joint = gameObject.GetComponent<HingeJoint>();
    }

    void Update()
    {
        JointSpring Sp = Joint.spring;
        var Angle = 0f;

        if (Sentido == Eixo.X)
            Angle = Target.localEulerAngles.x;//Direito
        if (Sentido == Eixo.Y)
            Angle = Target.localEulerAngles.y;//Cima
        if (Sentido == Eixo.Z)
            Angle = Target.localEulerAngles.z;//Frente

        if (Inverter)
            Angle *= -1;
        
        if (Angle < 180)
            Angle = Angle + 360;
        if (Angle > 180)
            Angle = Angle - 360;

        Angle += Compensador;
        Sp.targetPosition = Angle;
        Joint.spring = Sp;
    }
}
