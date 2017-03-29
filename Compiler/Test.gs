
GlobalFunction()
	XXClass obj = null

	a = 1 // local variable
	b = 2l
	c = a+b
	XXClass.abc = "abc"+"def"+1+1.1 // class static variable 
	XXClass.def = "sfjaklsdjf;asl"
				  "asdfasdfa"
				  "alsdf"
	obj.d = true // class instance variable
	a = Func(a,b,c)
	a,b = Func_(obj.d, XXClass.abc)

	if ()
	elseif ()
	else
	end

	loop()
	end

	loop
	until()
end

LocalFunction_()
	block

	end
	return 1,2
end
/*
class id:BaseClass(interface)
	int32 privateVariable_
	int64 publicVariableB = 0l

	id() // 构造，系统行为，不可访问
	end
	~id() // 析构，系统行为，不可访问
	end

	static
		bool staticPrivateVariable_

		id() // 静态构造，系统行为，不可访问
		end

		StaticPublicFunction()
		end
	end
end

interface
end

*/