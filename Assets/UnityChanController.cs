using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    //アニメーションをするためのコンポーネントを入れる
    private Animator myAnimator;
    //Unityちゃんを移動させるコンポーネントを入れる
    private Rigidbody myRigidbody;
    //前方向の速度
    private float velocityZ = 16f;
    //横方向の速度
    private float velocityX = 10f;
    //上方向の速度
    private float velocityY = 10f;
    //左右の移動できる範囲
    private float movableRange = 3.4f;

    // Start is called before the first frame update
    void Start()
    {
        //Animatorコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();

        //走るアニメーションを取得。今回のanimatorは"Speed"が0.8以上の時走ることになっている
        this.myAnimator.SetFloat("Speed", 0.8f);

        //Rigidbodyコンポーネントを取得
        this.myRigidbody = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update()
    {
        //横方向の入力による速度
        float inputVelocityX = 0;
        //上方向の入力による速度
        float inputVelocityY = 0;

        //Unityちゃんを矢印キーまたはボタンに応じて左右に移動させる
        if (Input.GetKey(KeyCode.LeftArrow) && -this.movableRange < this.transform.position.x)
        {
            inputVelocityX = -this.velocityX;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && this.transform.position.x < this.movableRange)
        {
            inputVelocityX = this.velocityX;
        }

        //ジャンプしていない時にスペースが押されたらジャンプする
        if (Input.GetKeyDown(KeyCode.Space) && this.transform.position.y < 0.5f)//連打するとJumpステート中にこの条件を満たしてしまう。JumpステートからLocomotionステートに遷移する条件は単なる時間経過であるため、Jumpステート中にこの条件を満たすとJumpステートの残り時間のアニメーションのまま動きが適用される。そのため、Jumpステートの残りアニメーション＋Locomotionステートの状態でジャンプアニメーションを起こしてしまう。
        {
            //ジャンプアニメを再生
            this.myAnimator.SetBool("Jump", true);
            //上方向への速度を代入
            inputVelocityY = this.velocityY;
            
        }
        else
        {
            //現在のY軸の速度を代入
            inputVelocityY = this.myRigidbody.velocity.y;
        }

        //Jumpステートの場合はJumpにfalseをセットする(GetCurrentAnimatorStateInfo(0).IsName("")で現在のアニメーションのステート名（JumpとかRunとか）と引数が一致しているかを返してくれる。
        if (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            this.myAnimator.SetBool("Jump", false);
        }

        //Unityちゃんに速度を与える（velocity(速度)は物理挙動を無視するため非推奨なのでは？)
        this.myRigidbody.velocity = new Vector3(inputVelocityX, inputVelocityY, this.velocityZ);
    }
}
