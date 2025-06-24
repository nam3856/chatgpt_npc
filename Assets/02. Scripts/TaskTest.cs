using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class TaskTest : MonoBehaviour
{
    // '비동기': '동기'의 반대말로 어떤 '작업'을 실행할 때 그 작업이 완료되지 않아도
    // 다음 작업을 진행할 수 있는 상태를 의미합니다.
    // '동기': 어떤 작업이 완료될 때까지 기다린 후 다음 작업을 진행하는 상태를 의미합니다.


    // 연산량이 많은 작업 (동기적으로 실행)
    int count = 0;
    private void LongLoop()
    {
        long sum = 1;
        for (int i = 1; i <= 1000000000; i++)
        {
            sum += i;

        }

        count++;
        Debug.Log($"{count}번째 sum: " + sum); // 결과 출력
    }

    // 비동기 (코루틴을 사용하여)
    private IEnumerator LongLoop_Coroutine()
    {
        long sum = 1;

        float startTime = Time.realtimeSinceStartup; // 시작 시간 기록
        for (int i = 1; i <= 1000000000; i++)
        {
            sum += i;
            if (i % 100000 == 0)
            {
                // 1000번마다 코루틴을 잠시 멈추고 다음 프레임으로 넘어갑니다.
                yield return null; // 이 줄에서 코루틴이 일시 중지되고, 다음 프레임에서 다시 실행됩니다.
            }
        }
        float endTime = Time.realtimeSinceStartup; // 종료 시간 기록
        count++;
        Debug.Log($"소요 시간 (코루틴): {endTime - startTime}초"); // 소요 시간 출력
    }

    // 비동기 (async/await를 사용하여)
    private async void LongLoop_Async()
    {
        long sum = 1;
        for (int i = 1; i <= 1000000000; i++)
        {
            sum += i;
            if (i % 100000 == 0)
            {
                // 비동기 작업을 잠시 멈추고 다음 프레임으로 넘어갑니다.
                await Task.Yield(); // 이 줄에서 비동기 작업이 일시 중지되고, 다음 프레임에서 다시 실행됩니다.
            }
        }
        count++;
        Debug.Log($"{count}번째 sum: " + sum); // 결과 출력
    }

    private int LongLoop2()
    {
        long sum = 1;

        for (int i = 1; i <= 1000000000; i++)
        {
            sum += i;
        }
        return 32323;

    }

    private async Awaitable<int> RefundAsync()
    {
        await Awaitable.BackgroundThreadAsync();
        LongLoop();
        return 8400;
    }

    private void OnGUI()
    {
        if (GUILayout.Button("대규모 작업 (동기)"))
        {
            LongLoop();
        }
        if (GUILayout.Button("대규모 작업 (비동기)"))
        {
            StartCoroutine(LongLoop_Coroutine());
        }
        if (GUILayout.Button("대규모 작업 (반환값이 없는 Task)"))
        {
            Task task1 = new Task(LongLoop);
            task1.Start();
        }
        if (GUILayout.Button("대규모 작업 (반환값이 있는 Task)"))
        {
            Task<int> task2 = new Task<int>(LongLoop2);
            task2.Start();
            task2.ContinueWith(t =>
            {
                Debug.Log($"반환값: {t.Result}");
            }); // 작업 완료 후 결과 출력
        }
    }

    public async Awaitable Start()
    {
        var refund = await RefundAsync();
        Debug.Log($"환불 금액: {refund}원");
    }
}
