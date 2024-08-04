using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    //動きを減衰させる係数
    private float coefficient = 0.99f;
    //ゲーム終了の判定
    private bool isEnd = false;
    //ゲーム終了時に表示するテキスト
    private GameObject stateText;
    //スコアを表示するテキスト
    private GameObject scoreText;
    //得点
    private int score = 0;
    //左ボタン押下の判定
    private bool isLButtonDown = false;
    //右ボタン押下の判定
    private bool isRButtonDown = false;
    //ジャンプボタン押下の判定
    private bool isJButtonDown = false;

    // Start is called before the first frame update
    void Start()
    {
        //Animatorコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();

        //走るアニメーションを取得。今回のanimatorは"Speed"が0.8以上の時走ることになっている
        this.myAnimator.SetFloat("Speed", 0.8f);

        //Rigidbodyコンポーネントを取得
        this.myRigidbody = GetComponent<Rigidbody>();

        //シーン中のstateTextオブジェクトを取得
        this.stateText = GameObject.Find("GameResultText");

        //シーン中のscoreTextオブジェクトを取得
        this.scoreText = GameObject.Find("ScoreText");

    }

    // Update is called once per frame
    void Update()
    {
        //ゲーム終了ならUnityちゃんの動きを減衰する
        if (this.isEnd)
        {
            this.velocityZ *= this.coefficient;
            this.velocityX *= this.coefficient;
            this.velocityY *= this.coefficient;
            this.myAnimator.speed *= this.coefficient;
        }

        //横方向の入力による速度
        float inputVelocityX = 0;
        //上方向の入力による速度
        float inputVelocityY = 0;

        //Unityちゃんを矢印キーまたはボタンに応じて左右に移動させる
        if ((Input.GetKey(KeyCode.LeftArrow)||this.isLButtonDown) && -this.movableRange < this.transform.position.x)
        {
            inputVelocityX = -this.velocityX;
        }
        else if ((Input.GetKey(KeyCode.RightArrow)||this.isRButtonDown) && this.transform.position.x < this.movableRange)
        {
            inputVelocityX = this.velocityX;
        }

        //ジャンプしていない時にスペースが押されたらジャンプする
        if ((Input.GetKeyDown(KeyCode.Space)||this.isJButtonDown) && this.transform.position.y < 0.5f&& this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))//連打するとJumpステート中にこの条件を満たしてしまう。JumpステートからLocomotionステートに遷移する条件は単なる時間経過であるため、Jumpステート中にこの条件を満たすとJumpステートの残り時間のアニメーションのまま動きが適用される。そのため、Jumpステートの残りアニメーション＋Locomotionステートの状態でジャンプアニメーションを起こしてしまう。追記：this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("")で条件を追加し、連打時の問題を解消
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

    //トリガーモードで他のオブジェクトと接触した場合の処理
    private void OnTriggerEnter(Collider other)
    {
        //障害物に衝突した場合
        if (other.gameObject.tag == "CarTag" || other.gameObject.tag == "TrafficConeTag")
        {
            this.isEnd = true;
            //stateTextにGAME OVERを表示
            this.stateText.GetComponent<Text>().text = "GAME OVER";
        }
        //ゴール地点に到達した場合
        if (other.gameObject.tag == "GoalTag")
        {
            this.isEnd = true;
            //stateTextにGAME CLEARを表示
            this.stateText.GetComponent<Text>().text = "CLEAR!!";
        }
        //コインに衝突した場合
        if (other.gameObject.tag == "CoinTag")
        {
            //スコアを加算
            this.score += 10;

            //ScoreTextに獲得した点数を表示
            this.scoreText.GetComponent<Text>().text = "Score" + this.score + "pt";

            //パーティクルを再生
            GetComponent<ParticleSystem>().Play();

            //接触したコインのオブジェクトを破壊
            Destroy(other.gameObject);
        }
    }

    //ジャンプボタンを押した場合の処理
    public void GetMyJumpButtonDown()
    {
        this.isJButtonDown = true;
    }
    //ジャンプボタンを離した場合の処理
    public void GetMyJumpButtonUp()
    {
        this.isJButtonDown = false;
    }

    //左ボタンを押し続けた場合の処理
    public void GetMyLeftButtonDown()
    {
        this.isLButtonDown = true;
    }
    //左ボタンを離した場合の処理
    public void GetMyLeftButtonUp()
    {
        this.isLButtonDown = false;
    }

    //右ボタンを押し続けた場合の処理
    public void GetMyRightButtonDown()
    {
        this.isRButtonDown = true;
    }
    //右ボタンを離した場合の処理
    public void GetMyRightButtonUp()
    {
        this.isRButtonDown = false;
    }
}
