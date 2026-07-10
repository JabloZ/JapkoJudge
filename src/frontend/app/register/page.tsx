'use client';
import { useActionState } from "react";
import { handleRegister } from "./actions";

export default function Register(){
    
    const[state,formAction,isPending]=useActionState(handleRegister,null);
    return(
            <form action={formAction}>
                
                <input type="text" name="username" placeholder="Username"/>
                <input type="email" name="email" placeholder="Email"/>
                <input type="password" name="password" placeholder="Password"/>
                <input type="password" name="password-verify" placeholder="Verify Password"/>
                <button type="submit">Register</button>
                {state?.message && (
                    <p style={{color:state.success? 'green': 'red'}}>
                        {state.message}
                    </p>
                )}

            </form>
        
    );
};