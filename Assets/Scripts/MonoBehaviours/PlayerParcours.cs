using UnityEngine;

public class PlayerParcours : MonoBehaviour
{
    public Transform rightHand, leftHand;

    [SerializeField] float m_GrabRange = 0.5f;
    [SerializeField] LayerMask m_GrabMask = 0;

    private PlayerController m_Controller;
    private Rigidbody m_RopeRigidbody;
    private Vector3 m_Offset;

    private void Awake()
    {
        m_Controller = GetComponent<PlayerController>();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!m_Controller.isGrabbing)
            {//Check if grabbable stuff is in range of hand
                Collider[] grabObjects = Physics.OverlapSphere(rightHand.position, m_GrabRange, m_GrabMask);
                if (grabObjects.Length > 0 && grabObjects[grabObjects.Length - 1] != null)
                {
                    //We got 'em. Attach player to rope stuff
                    int index = grabObjects.Length - 1;

                    //First, make sure the player stays on the exact same position as he was when the grab was found
                    m_Offset = grabObjects[index].transform.position - transform.position;
                    transform.SetParent(grabObjects[index].transform, true);

                    //Do IK stuff
                    m_Controller.SetIKOverrideTarget(grabObjects[index].transform);

                    //Do Physics stuff
                    m_RopeRigidbody = grabObjects[index].attachedRigidbody;
                    m_RopeRigidbody.velocity += m_Controller.rigidbody.velocity;
                    m_Controller.rigidbody.isKinematic = true;
                    m_Controller.isGrabbing = true;
                    SoundManager.PlaySoundAt("Swing", transform.position, 0.5f, Random.Range(0.9f, 1f));
                }
            }
            else
            {
                //Release rope grab
                transform.SetParent(null);
                m_Controller.SetIKOverrideTarget(null);
                m_Controller.rigidbody.isKinematic = false;
                m_Controller.rigidbody.velocity = m_RopeRigidbody.velocity;
                m_RopeRigidbody = null;
                m_Controller.isGrabbing = false;
                transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
                SoundManager.PlaySoundAt("Jump", transform.position, 0.05f, Random.Range(1f, 1.2f));
            }
        }
        if (m_Controller.isGrabbing)
        {
            transform.position = m_RopeRigidbody.transform.position - m_Offset;
            m_RopeRigidbody.AddForce(Vector3.right * Input.GetAxis("Horizontal") * 20);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        if (rightHand)
        {
            Gizmos.DrawWireSphere(rightHand.position, m_GrabRange);
        }
        Gizmos.color = Color.red;
        if (m_Controller && m_Controller.ikTarget)
        {
            if (m_Controller.overrideIKTarget == null)
            {
                Gizmos.DrawWireSphere(m_Controller.ikTarget.position, 0.1f);
            }
            else
            {
                Gizmos.DrawWireSphere(m_Controller.overrideIKTarget.position, 0.1f);
            }
        }
    }
}
