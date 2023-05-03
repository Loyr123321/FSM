<div><label class="login_input_log" @ref="ref1">Логин</label></div>

ElementReference ref1;
    ElementReference ref2;
    public void Enter(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            ref1.FocusAsync();
            ref2.FocusAsync();
            ref1.FocusAsync();

            if (isShowResetForm == false)
            {
                AuthAndLogin();
            }
            else
            {
                ResetPassword();
            }
        }
    }
	
	