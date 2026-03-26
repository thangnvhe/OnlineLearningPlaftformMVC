$(document).ready(function () {
    // Biến lưu trữ trạng thái câu trả lời
    let answeredQuestions = {};
    let currentQuestionId = null;

    // Thiết lập bộ đếm thời gian
    let quizTimeSeconds = parseInt($('#quiz-time').attr('data-seconds'));
    if (isNaN(quizTimeSeconds) || quizTimeSeconds <= 0) {
        quizTimeSeconds = 30 * 60; // Đặt mặc định là 30 phút nếu không có giá trị
    }

    let timeLeft = quizTimeSeconds;
    const timerElement = $('#timer');

    function updateTimerDisplay() {
        const minutes = Math.floor(timeLeft / 60);
        const seconds = timeLeft % 60;
        timerElement.text(
            (minutes < 10 ? '0' : '') + minutes + ':' +
            (seconds < 10 ? '0' : '') + seconds
        );

        // Chuyển màu khi thời gian còn ít
        if (timeLeft <= 60) { // 1 phút cuối
            timerElement.removeClass('bg-primary bg-warning').addClass('bg-danger');
        } else if (timeLeft <= 300) { // 5 phút cuối
            timerElement.removeClass('bg-primary').addClass('bg-warning');
        }
    }

    // Khởi chạy đồng hồ đếm ngược
    updateTimerDisplay(); // Hiển thị ban đầu

    const timerInterval = setInterval(function () {
        timeLeft--;
        updateTimerDisplay();

        if (timeLeft <= 0) {
            clearInterval(timerInterval);
            // Nộp bài khi hết thời gian
            submitQuiz();
        }
    }, 1000);

    // Kiểm tra và hiển thị câu hỏi đầu tiên
    function showFirstQuestion() {
        const firstQuestion = $('.question-container').first();
        if (firstQuestion.length) {
            firstQuestion.show();
            currentQuestionId = firstQuestion.attr('id').replace('question-', '');
            highlightCurrentQuestion(currentQuestionId);
        }
    }

    // Hàm đánh dấu câu hỏi hiện tại
    function highlightCurrentQuestion(questionId) {
        $('.question-nav-item').removeClass('active');
        $(`#question-nav-${questionId}`).addClass('active');
    }

    // Chuyển đổi giữa các câu hỏi khi nhấp vào danh sách
    $('.question-nav-item').click(function () {
        const questionId = $(this).data('question-id');
        showQuestion(questionId);
    });

    // Hàm hiển thị câu hỏi theo ID
    function showQuestion(questionId) {
        $('.question-container').hide();
        $(`#question-${questionId}`).show();
        currentQuestionId = questionId;
        highlightCurrentQuestion(questionId);
    }

    // Xử lý nút câu tiếp theo
    $('.next-question').click(function () {
        const nextId = $(this).data('next');
        if (nextId) {
            showQuestion(nextId);
        }
    });

    // Xử lý nút câu trước
    $('.prev-question').click(function () {
        const prevId = $(this).data('prev');
        if (prevId) {
            showQuestion(prevId);
        }
    });

    // Xử lý lưu câu trả lời
    $('.option-checkbox').change(function () {
        const questionId = $(this).data('question-id');

        // Đánh dấu câu hỏi đã được trả lời
        answeredQuestions[questionId] = true;

        // Cập nhật giao diện
        $(`#status-${questionId}`).html('<i class="fas fa-check text-success"></i>');
    });

    // Xử lý nút nộp bài
    $('#submit-quiz, #finish-button').click(function () {
        // Đếm số câu chưa trả lời
        const totalQuestions = $('.question-container').length;
        const answeredCount = Object.keys(answeredQuestions).length;
        const unansweredCount = totalQuestions - answeredCount;

        // Hiển thị modal xác nhận
        if (unansweredCount > 0) {
            $('#unanswered-warning').text(`Bạn còn ${unansweredCount} câu hỏi chưa trả lời. Bạn có chắc chắn muốn nộp bài?`);
        } else {
            $('#unanswered-warning').text('');
        }

        $('#submitConfirmModal').modal('show');
    });

    // Xử lý xác nhận nộp bài
    $('#confirm-submit').click(function () {
        submitQuiz();
    });

    // Hàm nộp bài
    function submitQuiz() {
        // Dừng đồng hồ đếm ngược
        clearInterval(timerInterval);

        // Thu thập câu trả lời
        let answers = [];
        $('.option-checkbox:checked').each(function () {
            answers.push({
                questionId: $(this).data('question-id'),
                optionId: $(this).val()
            });
        });
        function collectAllAnswers() {
            let answers = [];
            // Lặp qua tất cả các câu hỏi
            $('.question-container').each(function () {
                const questionId = $(this).attr('id').replace('question-', '');

                // Tìm tất cả các lựa chọn được chọn cho câu hỏi này
                let selectedOptions = [];
                $(this).find('.option-checkbox:checked').each(function () {
                    selectedOptions.push($(this).val());
                });

                // Thêm thông tin câu hỏi và câu trả lời vào mảng
                answers.push({
                    questionId: questionId,
                    selectedOptionIds: selectedOptions,
                    // Có thể thêm thông tin khác như trạng thái đã xem hay chưa
                    wasViewed: $(`#question-${questionId}`).is(':visible') || answeredQuestions[questionId] === true
                });
            });

            return answers;
        }
        function submitQuiz() {
            // Dừng đồng hồ đếm ngược
            clearInterval(timerInterval);

            // Thu thập tất cả câu trả lời
            let allAnswers = collectAllAnswers();

            // Thời gian đã sử dụng (tính bằng giây)
            let timeUsed = quizTimeSeconds - timeLeft;

            // Gửi câu trả lời đến server
            $.ajax({
                url: '@Url.Action("DoQuiz", "Result", new { area = "Mentee", quizId = quiz.QuizId })',
                type: 'POST',
                data: JSON.stringify({
                    quizId: '@Model.Quiz.QuizId',
                    answers: allAnswers,
                    timeUsed: timeUsed
                }),
                contentType: 'application/json',
                success: function (response) {
                    if (response.success) {
                        window.location.href = response.redirectUrl;
                    } else {
                        alert('Có lỗi xảy ra khi nộp bài: ' + response.message);
                    }
                },
                error: function () {
                    alert('Có lỗi xảy ra khi nộp bài. Vui lòng thử lại!');
                }
            });
        }
    }

    // Hiển thị câu hỏi đầu tiên khi trang đã tải xong
    showFirstQuestion();
});