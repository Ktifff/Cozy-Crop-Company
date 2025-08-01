using Game.Data;
using Game.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System;
using Game.Utils;
using System.Threading;
using System.Linq;
using Game.Audio;

namespace Game.UI
{
    public class OrdersMenu : MonoBehaviour, ISaveable
    {
        private const float OrderCooldown = 10f;
        private const int MaxOrders = 3;

        private static readonly int _isShownAnimatorHash = Animator.StringToHash("IsShown");

        private readonly List<ResourceRequirement> _orders = new();
        private readonly List<OrderData> _currectOrders = new();
        private readonly TaskQueue _orderCreationQueue = new ();

        [SerializeField] private OrderData[] _orderDatabase;
        [Space]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _content;
        [SerializeField] private TextMeshProUGUI _timeForNewOrderText;
        [SerializeField] private ResourceRequirement _resourceRequirementPrefab;

        private OrderCreator _orderCreator;
        private CancellationTokenSource _cancellationTokenSource;

        private bool _isVisible;

        public OrderData GetOrderByName(string name)
        {
            return _orderDatabase.FirstOrDefault(x => x.name == name);
        }

        public string Save()
        {
            OrdersSave save = new OrdersSave(_orderCreator, _currectOrders);
            return JsonUtility.ToJson(save, true);
        }

        public async void Load(string save)
        {
            if (!String.IsNullOrEmpty(save))
            {
                var json = JsonUtility.FromJson<OrdersSave>(save);
                for (int i = 0; i < json.Names.Length && _currectOrders.Count < MaxOrders; i++)
                {
                    OrderData order = GetOrderByName(json.Names[i]);
                    if (order != null)
                    {
                        _orders.Add(AddOrder(order));
                    }
                }
                if (_currectOrders.Count < MaxOrders)
                {
                    _orderCreationQueue.Enqueue(async () => { await AddNewOrderAsync(json.NextOrderTime, _cancellationTokenSource.Token); });
                }
                for (int i = _currectOrders.Count + 1; i < MaxOrders; i++)
                {
                    _orderCreationQueue.Enqueue(async () => { await AddNewOrderAsync(OrderCooldown, _cancellationTokenSource.Token); });
                }
            }
            else
            {
                for (int i = _currectOrders.Count; i < MaxOrders; i++)
                {
                    await AddNewOrderAsync(0, _cancellationTokenSource.Token);
                }
            }
        }

        public void SetVisibility(bool value)
        {
            _animator.SetBool(_isShownAnimatorHash, value);
            _isVisible = value;
        }

        private void Awake()
        {
            _cancellationTokenSource = new();
        }

        private void Update()
        {
            if (!_isVisible) return;
            if (_orderCreator is null)
            {
                _timeForNewOrderText.gameObject.SetActive(false);
            }
            else
            {
                _timeForNewOrderText.gameObject.SetActive(true);
                TimeSpan timeSpan = TimeSpan.FromSeconds(_orderCreator.Time);
                _timeForNewOrderText.text = $"Time to new order {timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }
        }

        private void OnDisable()
        {
            _orderCreationQueue.ClearQueue();
            _cancellationTokenSource.Cancel();
        }

        private ResourceRequirement AddOrder(OrderData order)
        {
            ResourceRequirement requirement = Instantiate(_resourceRequirementPrefab, _content);
            ResourceRequirement temp1 = requirement;
            OrderData temp2 = order;
            requirement.Init(order.Reward.Item.Icon, order.Name, order.Reward.Count, () => { CompleteOrder(temp1, temp2); }, () => { CancelOrder(temp1, temp2, OrderCooldown); });
            foreach (var item in order.OrderRequest)
            {
                requirement.AddRequestedResource(item.Item, item.Count);
            }
            _currectOrders.Add(order);
            return requirement;
        }

        private void CompleteOrder(ResourceRequirement resourceRequirement, OrderData data)
        {
            if (GameManager.PlayerInventory.UseRecipe(data.OrderRequest))
            {
                GameManager.PlayerInventory.AddItem(data.Reward.Item, data.Reward.Count);
                CancelOrder(resourceRequirement, data, 0);
                AudioMixerManager.PlayOrderSound();
            }
        }

        private async void CancelOrder(ResourceRequirement resourceRequirement, OrderData data, float cooldown)
        {
            if (!_orders.Contains(resourceRequirement)) return;
            _orders.Remove(resourceRequirement);
            Destroy(resourceRequirement.gameObject);
            if (!_currectOrders.Contains(data)) return;
            _currectOrders.Remove(data);
            if (cooldown == 0)
            {
                await AddNewOrderAsync(cooldown, _cancellationTokenSource.Token);
            }
            else
            {
                _orderCreationQueue.Enqueue(async () => { await AddNewOrderAsync(cooldown, _cancellationTokenSource.Token); });
            }
        }

        private async Task AddNewOrderAsync(float time, CancellationToken token)
        {
            _orderCreator = new OrderCreator(time);
            while (!_orderCreator.IsReady)
            {
                if (token.IsCancellationRequested) return;
                _orderCreator.MakeTick(Time.deltaTime);
                await Task.Yield();
            }
            _orderCreator = null;
            List<OrderData> orders = new();
            foreach (var order in _orderDatabase)
            {
                if (!_currectOrders.Contains(order))
                {
                    orders.Add(order);
                }
            }
            if (orders.Count == 0) return;
            if (token.IsCancellationRequested) return;
            _orders.Add(AddOrder(orders[UnityEngine.Random.Range(0, orders.Count)]));
        }

        public class OrderCreator
        {
            private float _time;

            public float Time => _time;
            public bool IsReady => _time <= 0;

            public OrderCreator(float time)
            {
                _time = time;
            }

            public void MakeTick(float delta)
            {
                _time -= delta;
            }
        }

        [Serializable]
        public class OrdersSave
        {
            public float NextOrderTime;
            public string[] Names;

            public OrdersSave(OrderCreator orderCreator, List<OrderData> currectOrders)
            {
                if (orderCreator != null)
                {
                    NextOrderTime = orderCreator.Time;
                }
                List<string> names = new();
                foreach(var data in currectOrders)
                {
                    names.Add(data.name);
                }
                Names = names.ToArray();
            }
        }
    }
}