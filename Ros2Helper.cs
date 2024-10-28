using System;
using System.Threading.Tasks;
using System.Threading;
using std_msgs.msg;

namespace ROS2
{
    public class Ros2Helper : IRos2Helper
    {
        private Node? _rosNode;
        private float _tickRate = 10.0f;
        private readonly CancellationTokenSource _cancelTokenSource = new();
        private CancellationToken _token;
        private Task? _spinnerTask;
        private Header _header = new();


        /// <summary>
        ///     Initializes the ros node.
        /// </summary>
        /// <param name="transformRootName">The root name of the node. Should be: this.transform.root.name</param>
        /// <param name="name">The name of the unity object. Should be: this.name</param>
        /// <param name="gameObjectInstanceId">The instance id of the game object. Should be: this.gameObject.GetInstanceID()</param>
        public void InitializeRos(string transformRootName, string name, int gameObjectInstanceId)
        {
            RCLdotnet.Init();
            string nodeName = transformRootName + "_" + name + "_" + Math.Abs(gameObjectInstanceId);
            this._rosNode = RCLdotnet.CreateNode(nodeName, transformRootName);

            this._token = this._cancelTokenSource.Token;
            this._spinnerTask = Task.Run(() => SpinRos(this._token), this._token);
        }

        /// <summary>
        ///     Adds the timestamp and frameId to the header.
        /// </summary>
        /// <param name="frameId">The frameId to put into the header.</param>
        /// <returns>A header containing the frameId and timestamp.</returns>
        public Header CreateHeader(string frameId)
        {
            var time = this._rosNode?.Clock.Now();
            _header.FrameId = frameId;
            if (time != null)
            {
                _header.Stamp.Sec = time.Sec;
                _header.Stamp.Nanosec = time.Nanosec;
            }

            return _header;
        }

        /// <summary>
        ///     Cancels the spinning of the node. Should be called from 
        /// </summary>
        public void CancelSpin()
        {
            this._cancelTokenSource.Cancel();
        }

        private async Task SpinRos(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (RCLdotnet.Ok())
                {
                    RCLdotnet.SpinOnce(_rosNode, 500);
                    await Task.Delay(TimeSpan.FromMilliseconds(1), token);
                }
            }
        }

        public Subscription<T>? CreateSubscription<T>(string topic, Action<T> callback, QosProfile? qosProfile = null) where T : IRosMessage, new()
        {
            return _rosNode?.CreateSubscription<T>(topic, callback, qosProfile);
        }

        public Publisher<T>? CreatePublisher<T>(string topic, QosProfile? qosProfile = null) where T : IRosMessage
        {
            return _rosNode?.CreatePublisher<T>(topic, qosProfile);
        }

        public Service<TService, TRequest, TResponse>? CreateService<TService, TRequest, TResponse>(string serviceName, Action<TRequest, TResponse> callback)
            where TService : IRosServiceDefinition<TRequest, TResponse>
            where TRequest : IRosMessage, new()
            where TResponse : IRosMessage, new()
        {
            return _rosNode?.CreateService<TService, TRequest, TResponse>(serviceName, callback);
        }

        public Client<TService, TRequest, TResponse>? CreateClient<TService, TRequest, TResponse>(string serviceName)
            where TService : IRosServiceDefinition<TRequest, TResponse>
            where TRequest : IRosMessage, new()
            where TResponse : IRosMessage, new()
        {
            return _rosNode?.CreateClient<TService, TRequest, TResponse>(serviceName);
        }

        public Timer? CreateTimer(TimeSpan period, Action<TimeSpan> callback)
        {
            return _rosNode?.CreateTimer(period, callback);
        }

        public GuardCondition? CreateGuardCondition(Action callback)
        {
            return _rosNode?.CreateGuardCondition(callback);
        }

        public ActionClient<TAction, TGoal, TResult, TFeedback>? CreateActionClient<TAction, TGoal, TResult, TFeedback>(string actionName)
            where TAction : IRosActionDefinition<TGoal, TResult, TFeedback>
            where TGoal : IRosMessage, new()
            where TResult : IRosMessage, new()
            where TFeedback : IRosMessage, new()
        {
            return _rosNode?.CreateActionClient<TAction, TGoal, TResult, TFeedback>(actionName);
        }

        public ActionServer<TAction, TGoal, TResult, TFeedback>? CreateActionServer<TAction, TGoal, TResult, TFeedback>(string actionName, Action<ActionServerGoalHandle<TAction, TGoal, TResult, TFeedback>> acceptedCallback, Func<Guid, TGoal, GoalResponse>? goalCallback = null, Func<ActionServerGoalHandle<TAction, TGoal, TResult, TFeedback>, CancelResponse>? cancelCallback = null)
            where TAction : IRosActionDefinition<TGoal, TResult, TFeedback>
            where TGoal : IRosMessage, new()
            where TResult : IRosMessage, new()
            where TFeedback : IRosMessage, new()
        {
            return _rosNode?.CreateActionServer<TAction, TGoal, TResult, TFeedback>(actionName, acceptedCallback,
                goalCallback, cancelCallback);
        }

        public void SetTickRate(float tickRate)
        {
            this._tickRate = tickRate;
        }

        public float GetTickRate()
        {
            return this._tickRate;
        }

        /// <summary>
        ///     Make quaternion in ROS from Unity, accounting for axes transformation.
        ///     See `OrientationUnityToRos()`.
        /// </summary>
        public geometry_msgs.msg.Quaternion GetRosQuaternionFromUnity(float x, float y, float z, float w)
        {
            geometry_msgs.msg.Quaternion output = new()
            {
                X = z,
                Y = -x,
                Z = y,
                W = -w
            };
            return output;
        }

        /// <summary>
        ///     Make vector in ROS from Unity, accounting for axes transformation.
        ///     See `AxesUnityToRos()`.
        /// </summary>
        public geometry_msgs.msg.Vector3 GetRosVectorFromUnity(float x, float y, float z)
        {
            geometry_msgs.msg.Vector3 output = new()
            {
                X = z,
                Y = -x,
                Z = y
            };
            return output;
        }

    }
}
