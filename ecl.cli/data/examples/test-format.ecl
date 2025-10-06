{
    User = @env("USERNAME" "default_user")
    UnreadMessages = 5,
    Greeting = $"Hello, {@env("USER")}! You have {UnreadMessages} unread messages."
}

