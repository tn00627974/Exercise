document.addEventListener('DOMContentLoaded', () => {
    // DOM Elements
    const createGiftBtn = document.getElementById('create-gift-btn');
    const modal = document.getElementById('modal');
    const closeBtn = document.querySelector('.close-btn');
    const confirmBtn = document.getElementById('confirm-btn');
    const messageInput = document.getElementById('message-input');
    const giftContainer = document.getElementById('gift-container');
    const charCount = document.querySelector('.char-count');

    // Assets
    const giftImages = [
        'assets/gift_box.png',
        'assets/red_envelope.png',
        'assets/lantern.png',
        'assets/lucky_bag.png'
    ];

    // State
    let isModalOpen = false;

    // Event Listeners
    createGiftBtn.addEventListener('click', openModal);
    closeBtn.addEventListener('click', closeModal);
    confirmBtn.addEventListener('click', createGift);
    
    // Close modal when clicking outside
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            closeModal();
        }
    });

    // Character count update
    messageInput.addEventListener('input', () => {
        const currentLength = messageInput.value.length;
        charCount.textContent = `${currentLength}/100`;
    });

    // Functions
    function openModal() {
        modal.classList.remove('hidden');
        isModalOpen = true;
        messageInput.focus();
    }

    function closeModal() {
        modal.classList.add('hidden');
        isModalOpen = false;
        messageInput.value = ''; // Clear input
        charCount.textContent = '0/100';
    }

    function createGift() {
        const message = messageInput.value.trim();
        
        if (!message) {
            alert('請輸入祝福內容喔！');
            return;
        }

        // Randomly select a gift image
        const randomImage = giftImages[Math.floor(Math.random() * giftImages.length)];

        // Create gift element
        const giftEl = document.createElement('div');
        giftEl.classList.add('gift');
        
        // Random position
        // Avoid placing gifts too close to the title/button area (top 30%)
        // Keep within viewport bounds (padding 50px)
        const maxX = window.innerWidth - 100; // Gift width approx 80px + padding
        const maxY = window.innerHeight - 100;
        const minY = window.innerHeight * 0.3; // Start from 30% down

        const randomX = Math.random() * (maxX - 50) + 25;
        const randomY = Math.random() * (maxY - minY) + minY;

        giftEl.style.left = `${randomX}px`;
        giftEl.style.top = `${randomY}px`;

        // Inner HTML structure
        giftEl.innerHTML = `
            <img src="${randomImage}" alt="Gift">
            <div class="gift-tooltip">${escapeHtml(message)}</div>
        `;

        // Add click event to toggle tooltip on mobile (or desktop)
        giftEl.addEventListener('click', (e) => {
            e.stopPropagation(); // Prevent bubbling
            // Toggle active class for mobile touch support
            document.querySelectorAll('.gift').forEach(g => {
                if (g !== giftEl) g.classList.remove('active');
            });
            giftEl.classList.toggle('active');
        });

        // Append to container
        giftContainer.appendChild(giftEl);

        // Close modal
        closeModal();
    }

    // Helper to prevent XSS
    function escapeHtml(text) {
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.replace(/[&<>"']/g, function(m) { return map[m]; });
    }

    // Close active tooltips when clicking elsewhere
    document.addEventListener('click', () => {
        document.querySelectorAll('.gift').forEach(g => g.classList.remove('active'));
    });
});
