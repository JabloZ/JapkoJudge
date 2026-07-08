

export default function Register(){
    
    async function handleRegister(formData:FormData){
        "use server";
        const username=formData.get("username") as string;
        const email=formData.get("email") as string;
        const password1=formData.get("password1") as string;
        const password2=formData.get("password2") as string;
        //TODO: verify
        //TODO: send form to backend and wait for response
        const response=await fetch("http://judge-backend:8001/api/register",{
            method:"POST",
            headers: {"Content-Type":"application/json"},
            body: JSON.stringify({username,email,password1})
        });
    }
    return(
        
            <form action={handleRegister}>
                <input type="text" name="username" placeholder="Username"/>
                <input type="email" name="email" placeholder="Email"/>
                <input type="password" name="password1" placeholder="Password"/>
                <input type="password" name="password2" placeholder="Verify Password"/>
                <button type="submit">Register</button>
            </form>
        
    );
};