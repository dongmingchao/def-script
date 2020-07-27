module Tests
let target = """
var_str = 'str'
var_number = 12

var_obj1 = { key1: 'val1' }
var_obj2 = { key1: 'val1', key2: 1 }
var_obj3 = { key1: 'val1', key2: 1, key3: true }
var_obj4 = { key1: 'val1', key2: 1, nest_key: { key3: true } }
var_obj5 = { key1: 'val1', key2: 1, nest_key: { key3: true, key2: 1 }, nest_key2: { key_bool: false } }
var_obj6 =
	key2: 'val2'
	number_key: 1
	bool_key: true
	nest_key:
		key3: 'val3'

func(args1, args2) =>
	local_var_number = 34
	local_var_bool = false
	local_var_str = 'local str()asdsadw[].{}:Dsa12=>'
	local_equal = args1

var_assign_func = func 1

var_boolean = true
func2({ arg_key }) =>
  ret arg_key
  local_rest_var = 1

var_obj3 = { key1: 'val1', key2: 1, key3: true }
func(args1, args2) =>
	local_var_number = 34
	var_obj4 = { key1: 'val1', key2: 1, nest_key: { key3: true, key2: 1 }, nest_key2: { key_bool: false } }

func 1, '123'
func2 (func 1, '123')
func2 (func 1), '123'

if var_boolean
  func var_boolean
else func 2

switch var_number
  case 1
    func 1
  case 2
    func 2
    fallthrough
  case 3
    func 3
"""
