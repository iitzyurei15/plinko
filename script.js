const multipliers = [5, 3, 2, 1.2, 0.4, 1.2, 2, 3, 5];
const weights = [3, 7, 12, 18, 20, 18, 12, 7, 3];

let balance = 1000;
let isDropping = false;

const balanceEl = document.getElementById('balance');
const homeSection = document.getElementById('home');
const gameSection = document.getElementById('game');
const startBtn = document.getElementById('start-btn');
const homeBtn = document.getElementById('home-btn');
const dropBtn = document.getElementById('drop-btn');
const betInput = document.getElementById('bet');
const slotsEl = document.getElementById('slots');
const resultEl = document.getElementById('result');
const ballEl = document.getElementById('ball');

function renderSlots() {
  slotsEl.innerHTML = '';
  multipliers.forEach((value) => {
    const slot = document.createElement('div');
    slot.className = 'slot';
    slot.textContent = `${value}x`;
    slotsEl.appendChild(slot);
  });
}

function goToGame() {
  homeSection.classList.add('hidden');
  gameSection.classList.remove('hidden');
}

function goHome() {
  gameSection.classList.add('hidden');
  homeSection.classList.remove('hidden');
}

function updateBalance(nextValue) {
  balance = Math.max(0, Math.floor(nextValue));
  balanceEl.textContent = balance;
}

function pickSlotIndex() {
  const total = weights.reduce((sum, w) => sum + w, 0);
  let roll = Math.random() * total;

  for (let i = 0; i < weights.length; i += 1) {
    roll -= weights[i];
    if (roll <= 0) {
      return i;
    }
  }

  return weights.length - 1;
}

function clearHighlights() {
  [...slotsEl.children].forEach((slot) => slot.classList.remove('highlight'));
}

function dropBall() {
  if (isDropping) return;

  const bet = Number(betInput.value);
  if (!Number.isFinite(bet) || bet < 1) {
    resultEl.textContent = 'Please enter a valid bet of at least 1 coin.';
    return;
  }

  if (bet > balance) {
    resultEl.textContent = 'Not enough coins. Lower your bet.';
    return;
  }

  isDropping = true;
  clearHighlights();
  updateBalance(balance - bet);

  ballEl.classList.remove('hidden', 'drop');
  ballEl.style.left = '50%';
  void ballEl.offsetWidth;
  ballEl.classList.add('drop');

  const index = pickSlotIndex();
  const multiplier = multipliers[index];
  const payout = Math.floor(bet * multiplier);

  setTimeout(() => {
    const slotWidthPercent = 100 / multipliers.length;
    ballEl.style.left = `${slotWidthPercent * index + slotWidthPercent / 2}%`;
    slotsEl.children[index].classList.add('highlight');
  }, 750);

  setTimeout(() => {
    updateBalance(balance + payout);
    const net = payout - bet;
    const netPrefix = net >= 0 ? '+' : '';
    resultEl.textContent = `Landed on ${multiplier}x. You won ${payout} coins (${netPrefix}${net}).`;
    isDropping = false;
  }, 1200);
}

startBtn.addEventListener('click', goToGame);
homeBtn.addEventListener('click', goHome);
dropBtn.addEventListener('click', dropBall);

renderSlots();
