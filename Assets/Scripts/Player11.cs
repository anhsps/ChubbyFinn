using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player11 : MonoBehaviour
{
    [SerializeField] private Button upBtn, downBtn, leftBtn, rightBtn;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveDis = 1f;

    Animator animator;
    private Vector3 targetPos;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canMove2 = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        targetPos = transform.position;

        upBtn.onClick.AddListener(() => UpdatePos(Vector3.up));
        downBtn.onClick.AddListener(() => UpdatePos(Vector3.down));
        leftBtn.onClick.AddListener(() => UpdatePos(Vector3.left));
        rightBtn.onClick.AddListener(() => UpdatePos(Vector3.right));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = MoveTo(transform.position, targetPos);
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            canMove = true;
        }

        if (canMove && canMove2) MoveInput();
    }

    void MoveInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) UpdatePos(Vector3.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) UpdatePos(Vector3.down);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) UpdatePos(Vector3.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) UpdatePos(Vector3.right);
    }

    private void UpdatePos(Vector3 direction)
    {
        //Debug.DrawRay(targetPos, direction, Color.red, 5f);
        if (!canMove || Physics2D.Raycast(targetPos, direction, moveDis, LayerMask.GetMask("Ground"))) return;

        RaycastHit2D hit = Physics2D.Raycast(targetPos, direction, moveDis, LayerMask.GetMask("Ball"));
        if (hit.collider)
        {
            Vector3 nextBallPos = hit.transform.position + direction * moveDis;

            // tat Composite Collider2D ms hoat dong
            // ball > Collider2D > Rigdbody2D (Kinematic) , Dynamic ko chinh xac
            //if (Physics2D.OverlapCircle(nextBallPos, 0.1f, LayerMask.GetMask("Ground", "Ball"))) return;
            if (Physics2D.OverlapPoint(nextBallPos, LayerMask.GetMask("Ground", "Ball"))) return;

            canMove2 = false;
            StartCoroutine(MoveBall(hit.collider.transform, nextBallPos));
        }

        SoundManager11.Instance.PlaySound(4);

        string nameBool = (direction == Vector3.up) ? "Up" : (direction == Vector3.down) ? "Down" : "Run";
        animator.Rebind();
        animator.SetBool(nameBool, true);
        if ((direction == Vector3.left && facingRight) || (direction == Vector3.right) && !facingRight)
            RotateScaleX();
        targetPos += direction * moveDis;
        canMove = false;
    }

    private Vector3 MoveTo(Vector3 from, Vector3 to) => Vector3.MoveTowards(from, to, moveSpeed * Time.deltaTime);

    private IEnumerator MoveBall(Transform ball, Vector3 to)
    {
        while (Vector3.Distance(ball.position, to) >= 0.01f)
        {
            ball.position = MoveTo(ball.position, to);
            yield return null;
        }
        ball.position = to;

        GameController11.Instance.CheckWin();
        canMove2 = true;
    }

    [SerializeField] private bool facingRight;
    private void RotateScaleX()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
